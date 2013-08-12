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

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LpsRetsClient.Transaction;

namespace LpsRetsClient.Tests
{
	[TestClass]
	public class PostObjectTest
	{
		private const string PhotoFilePath = @"C:\Projects\Data\100_1630.JPG";
		private const string AltPhotoFilePath = @"C:\Projects\Data\img\big_image.jpg";
		private const string UserName = "xxx";
		private const string Password = "xxx";
		private const string BaseUrl = "http://localhost";
		private const string LoginUrl = "/RETS/FNISRETS.aspx/XXX/login";
		private const int Port = 80;
		private const string UserAgent = "reInsightMobile/1.0";
		private const string UserAgentAuth = "xxx";
		private const string RetsVersion = "RETS/1.5";
		private const AuthenticationMethod AuthMethod = AuthenticationMethod.Digest;

		private RetsSession Session { get; set; }
		private RetsLoginResponse LoginResponse { get; set; }

		[TestInitialize]
		public void Initialize()
		{
			Session = RetsSession.Create(BaseUrl, LoginUrl, Port, UserName, Password, UserAgent, UserAgentAuth, RetsVersion, AuthMethod);
			LoginResponse = Session.Login();
		}

		[TestCleanup]
		public void Cleanup()
		{
			if (LoginResponse != null && Session != null && LoginResponse.IsSuccessful)
				Session.Logout();
		}

		[TestMethod]
		public void PostObjectAdd()
		{
			var fileInfo = new FileInfo(PhotoFilePath);
			var fileContents = File.ReadAllBytes(PhotoFilePath);
			Session.PostObject(RetsPostObjectParams.ActionAdd("image/jpeg", "Photo", "Property", "1234", null, fileInfo.Name, fileContents));
		}

		[TestMethod]
		public void PostObjectReplace()
		{
			var fileInfo = new FileInfo(PhotoFilePath);
			var fileContents = File.ReadAllBytes(AltPhotoFilePath);
			Session.PostObject(RetsPostObjectParams.ActionReplace("image/jpeg", "Photo", "Property", "1234", 1, fileInfo.Name, fileContents));
		}

		[TestMethod]
		public void PostObjectDelete()
		{
			Session.PostObject(RetsPostObjectParams.ActionDelete("image/jpeg", "Photo", "Property", "1234", 2));
		}
	}
}
