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
using System.Net;
using LpsRetsClient.Data;
using LpsRetsClient.Security;
using LpsRetsClient.Transaction;

namespace LpsRetsClient.Http
{
	public class RetsRequest
	{
		public string Post(IRetsRequest retsRequest)
		{
			var methodUri = new Uri(retsRequest.MethodUrl);
			var sessionInfo = retsRequest.Session;

			var networkCredential = new NetworkCredential(sessionInfo.UserName, sessionInfo.Password);
			var authMethod = retsRequest.Session.AuthMethod == AuthenticationMethod.Basic ? "Basic" : "Digest";
			var credentials = new CredentialCache {{methodUri, authMethod, networkCredential}};

			var request = (HttpWebRequest)WebRequest.Create(methodUri);
			request.PreAuthenticate = true;
			request.Credentials = credentials;
			request.UserAgent = sessionInfo.UserAgent;
			request.Headers.Add("RETS-Version", sessionInfo.RetsVersion);
			request.Method = "POST";
			request.ContentType = "text/xml";
			request.CookieContainer = sessionInfo.Cookies ?? new CookieContainer();

			if (!string.IsNullOrWhiteSpace(sessionInfo.UserAgentAuth))
				request.Headers.Add("RETS-UA-Authorization", GetUserAgentAuthorization(sessionInfo));

			foreach (var header in retsRequest.Headers)
				request.Headers.Add(header.Key, header.Value);

			// Handle Digest calculations here because PreAuthenticate doesn't work properly for digest
			if (!string.IsNullOrWhiteSpace(sessionInfo.Authorization))
			{
				var digestUtility = new DigestUtility();
				var newHeader = digestUtility.UpdateAuthorizationHeader(sessionInfo.Authorization, methodUri.AbsolutePath, sessionInfo.Password, request.Method);
				if (!string.IsNullOrWhiteSpace(newHeader))
					request.Headers.Add("Authorization", newHeader);
			}

			// Prepare the request after everything has been set up, but before we make the call
			// so the request can be modified as needed for the transaction.
			// NOTE: Set headers before streaming PostBody data
			retsRequest.PrepareRequest(request);

			byte[] rawPostData = retsRequest.PostBody;
			if (rawPostData != null && rawPostData.Length > 0)
			{
				request.ContentLength = rawPostData.Length;
				
				using (var requestStream = request.GetRequestStream())
				{
					requestStream.Write(rawPostData, 0, rawPostData.Length);
					requestStream.Close();
				}
			}
			else
			{
				request.ContentLength = 0;
			}


			HttpWebResponse response = null;
			
			try
			{
				response = (HttpWebResponse)request.GetResponse();

				// Store the authorization header value for subsequent use
				sessionInfo.Authorization = request.Headers.Get("Authorization");
				
				sessionInfo.Cookies = new CookieContainer();
				foreach (Cookie cookie in response.Cookies)
					sessionInfo.Cookies.Add(cookie);

				using (var stream = new StreamReader(response.GetResponseStream()))
				{
					string result = stream.ReadToEnd();

					return result;
				}
			}
			finally
			{
				if (response != null)
				{
					response.Close();
				}
			}
		}

		/// <summary>
		/// a1 ::= MD5( product ":" UserAgent-Password )
		/// ua-digest-response ::= LHEX( MD5( LHEX(a1)":" RETS-Request-ID ":" session-id ":" version-info))
		/// </summary>
		/// <param name="sessionInfo"></param>
		/// <returns></returns>
		private string GetUserAgentAuthorization(SessionInfo sessionInfo)
		{
			string a1 = Crypto.GetMd5(sessionInfo.UserAgent + ":" + sessionInfo.UserAgentAuth);

			return "Digest " + Crypto.GetMd5(a1 + ":" + (sessionInfo.RetsRequestId ?? string.Empty) + ":" + (sessionInfo.RetsSessionId ?? string.Empty) + ":" + sessionInfo.RetsVersion);
		}
	}
}

