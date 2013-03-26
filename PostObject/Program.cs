#region LICENSE
//Copyright (c) 2013 LPS MLS Solutions

//Permission is hereby granted, free of charge, to any person obtaining a copy of
//this software and associated documentation files (the "Software"), to deal in
//the Software without restriction, including without limitation the rights to
//use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
//of the Software, and to permit persons to whom the Software is furnished to do
//so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
#endregion

using System;
using System.IO;
using Args;
using Args.Help;
using Args.Help.Formatters;
using Seterlund.CodeGuard;
using LpsRetsClient;
using LpsRetsClient.Transaction;

namespace PostObject
{
	class Program
	{
		static void Main(string[] args)
		{
			IModelBindingDefinition<ProgramOptions> configuration = null;
			ProgramOptions options = null;

			try
			{
				configuration = Configuration.Configure<ProgramOptions>();
				options = configuration.CreateAndBind(args);
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteFail(ex.Message.Replace("{0}", "{{0}}"));
				OutputHelp(configuration);
				Environment.Exit(ExitCode.BadArguments);
			}

			EnsureArgumentsValid(options, configuration);

			string password = PromptForPassword();

			var uri = new Uri(options.LoginUrl);
			
			var session = RetsSession.Create(uri.GetLeftPart(UriPartial.Authority), uri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped), options.Port, options.UserName, password, options.UserAgent, options.UserAgentAuth, options.RetsVersion);

			var loginResponse = Login(options, session);
			if (loginResponse.IsSuccessful)
			{
				ConsoleHelper.WriteOk("Success!");

				byte[] fileContents = null;

				if (!string.IsNullOrWhiteSpace(options.File))
					fileContents = File.ReadAllBytes(options.File);

				var response = Execute(options, session, fileContents);
				if (response == null)
				{
					ConsoleHelper.WriteFail("Failed.");
					Logout(session);
					Environment.Exit(ExitCode.Fail);
				}

				if (response.IsSuccessful)
				{
					ConsoleHelper.WriteOk("Success!");
					ConsoleHelper.WriteOk("RETS Reply Code {0}: {1}", response.ReplyCode, response.ReplyText);
				}
				else
				{
					ConsoleHelper.WriteFail("Failed.");
					ConsoleHelper.WriteFail("RETS Reply Code {0}: {1}", response.ReplyCode, response.ReplyText);
				}

				if (options.Verbose)
					Console.WriteLine(response.Response.OuterXml);

				Logout(session);

				Environment.Exit(response.IsSuccessful ? ExitCode.Success : ExitCode.Fail);
			}
			else
			{
				ConsoleHelper.WriteFail("Failed.");
				ConsoleHelper.WriteFail("Code {0}: {1}", loginResponse.ReplyCode, loginResponse.ReplyText);
				Environment.Exit(ExitCode.Fail);
			}
		}

		private static RetsLoginResponse Login(ProgramOptions options, RetsSession session)
		{
			ConsoleHelper.WriteInfo("Logging in to {0}...", options.LoginUrl);
			
			return session.Login();
		}

		private static void Logout(RetsSession session)
		{
			ConsoleHelper.WriteInfo("Logging out...");
			session.Logout();
		}

		private static RetsPostObjectResponse Execute(ProgramOptions options, RetsSession session, byte[] fileContents)
		{
			RetsPostObjectResponse response = null;

			ConsoleHelper.WriteInfo("Executing action {0} {1} {2} for ID {3}...", options.Action, options.Resource, options.Type, options.ResourceId);

			try
			{
				string fileName = null;
				if (!string.IsNullOrWhiteSpace(options.File))
				{
					var fileInfo = new FileInfo(options.File);
					fileName = fileInfo.Name;
				}

				if (options.Action.Equals("Add", StringComparison.InvariantCultureIgnoreCase))
					response = session.PostObject(RetsPostObjectParams.ActionAdd(options.ContentType, options.Type, options.Resource, options.ResourceId, options.ObjectId, fileName, fileContents, options.Description, options.Label, options.Accessibility, options.DelegateId, options.DelegateHash, options.DelegatePassword));
				else if (options.Action.Equals("Replace", StringComparison.InvariantCultureIgnoreCase))
					response = session.PostObject(RetsPostObjectParams.ActionReplace(options.ContentType, options.Type, options.Resource, options.ResourceId, options.ObjectId ?? -1, fileName, fileContents, options.Description, options.Label, options.Accessibility, options.DelegateId, options.DelegateHash, options.DelegatePassword));
				else if (options.Action.Equals("Delete", StringComparison.InvariantCultureIgnoreCase))
					response = session.PostObject(RetsPostObjectParams.ActionDelete(options.ContentType, options.Type, options.Resource, options.ResourceId, options.ObjectId, options.DelegateId, options.DelegateHash, options.DelegatePassword));
				else
					response = session.PostObject(new RetsPostObjectParams
					{
						ContentType = options.ContentType,
						Accessibility = options.Accessibility,
						ContentDescription = options.Description,
						ContentLabel = options.Label,
						FileContents = fileContents,
						ObjectId = options.ObjectId,
						Resource = options.Resource,
						ResourceId = options.ResourceId,
						Type = options.Type,
						UpdateAction = options.Action,
						DelegateId = options.DelegateId,
						DelegateHash = options.DelegateHash,
						DelegatePassword = options.DelegatePassword
					});
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteFail(ex.Message);
				Environment.Exit(ExitCode.Fail);
			}

			return response;
		}

		private static void EnsureArgumentsValid(ProgramOptions options, IModelBindingDefinition<ProgramOptions> configuration)
		{
			try
			{
				Guard.That(options.Port).IsTrue(p => p > 0, "Port must be greater than 0.");

				// Replace requires an object ID
				if (options.Action.Equals("Replace", StringComparison.InvariantCultureIgnoreCase))
					Guard.That(options.ObjectId).IsTrue(o => o != null, "Object ID must be supplied when action is Replace.");

				// Add and Replace require a file to upload and a description for documents
				if (options.Action.Equals("Add", StringComparison.InvariantCultureIgnoreCase) || options.Action.Equals("Replace", StringComparison.InvariantCultureIgnoreCase))
				{
					Guard.That(options.File).IsTrue(f => !string.IsNullOrWhiteSpace(f), "File must be provided when action is Add or Replace.");

					var fileInfo = new FileInfo(options.File);
					if (!fileInfo.Exists)
						throw new ArgumentException("Invalid file specified.");

					if (options.Type.Equals("Document", StringComparison.InvariantCultureIgnoreCase))
						Guard.That(options.Description).IsTrue(d => !string.IsNullOrWhiteSpace(d), "Description must be provided when type is Document and action is Add or Replace.");
				}

				if (!string.IsNullOrWhiteSpace(options.DelegateId))
				{
					Guard.That(options.DelegateHash).IsTrue(dh => !string.IsNullOrWhiteSpace(dh), "Delegate user hash is required when /did parameter is used.");
					Guard.That(options.DelegatePassword).IsTrue(dp => !string.IsNullOrWhiteSpace(dp), "Delegate authorization password is required when /did parameter is used.");
				}
			}
			catch (Exception ex)
			{
				ConsoleHelper.WriteFail(ex.Message);
				OutputHelp(configuration);
				Environment.Exit(ExitCode.BadArguments);
			}
		}

		private static void OutputHelp(IModelBindingDefinition<ProgramOptions> configuration)
		{
			var help = new HelpProvider();
			var helpProvider = help.GenerateModelHelp(configuration);

			var consoleFormatter = new ConsoleHelpFormatter(80, 1, 5);
			Console.WriteLine(consoleFormatter.GetHelp(helpProvider));
		}

		public static class ExitCode
		{
			public const int Success = 0;
			public const int BadArguments = 1;
			public const int InvalidPath = 2;
			public const int Fail = 255;
		}

		// Lifted from comments @ http://www.codeproject.com/Articles/8110/NET-Console-Password-Input-By-Masking-Keyed-In-Ch
		private static string PromptForPassword()
		{
			Console.Write("Password: ");

			string pwd = default(string);
			byte positionStart = (byte)Console.CursorLeft, positionNow = (byte)Console.CursorLeft;
			ConsoleKeyInfo cki;
			do
			{
				cki = Console.ReadKey(true);
				switch (cki.Key)
				{
					case ConsoleKey.Backspace:
						if (pwd != null && Console.CursorLeft > positionStart)
						{
							pwd = pwd.Remove(Console.CursorLeft - positionStart - 1, 1);
							Console.CursorLeft = pwd.Length + positionStart;
							Console.Write(" ");
							Console.CursorLeft = positionNow - 1;
							positionNow = (byte)Console.CursorLeft;
						}
						break;
					case ConsoleKey.LeftArrow:
						if (Console.CursorLeft != 0 && Console.CursorLeft > positionStart)
						{
							Console.CursorLeft--;
							positionNow = (byte)Console.CursorLeft;
						}
						break;
					case ConsoleKey.RightArrow:
						if (pwd != null && positionNow - positionStart < pwd.Length)
						{
							Console.CursorLeft++;
							positionNow = (byte)Console.CursorLeft;
						}
						break;
					case ConsoleKey.Delete:
						if (pwd != null && Console.CursorLeft < pwd.Length + positionStart)
						{
							pwd = pwd.Remove(Console.CursorLeft - positionStart, 1);
							Console.CursorLeft = pwd.Length + positionStart;
							Console.Write(" ");
							Console.CursorLeft = positionNow;
						}
						break;
					default:
						if (!char.IsControl(cki.KeyChar))
						{
							if (pwd != null && Console.CursorLeft < pwd.Length + positionStart)
							{
								Console.CursorLeft = pwd.Length + positionStart;
								Console.Write("*");
								Console.CursorLeft = positionNow + 1;
								positionNow = (byte)Console.CursorLeft;
								pwd = pwd.Insert(positionNow - positionStart - 1, cki.KeyChar.ToString());
							}
							else
							{
								Console.Write("*");
								positionNow = (byte)Console.CursorLeft;
								pwd += cki.KeyChar.ToString();
							}
						}
						break;
				}
			}
			while (cki.Key != ConsoleKey.Enter);

			Console.WriteLine();

			return pwd;
		}
	}
}
