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
using LpsRetsClient.Data;
using LpsRetsClient.Http;
using LpsRetsClient.Transaction;

namespace LpsRetsClient
{
	public class RetsSession
	{
		private SessionInfo SessionInfo { get; set; }
		public RetsLoginResponse LoginInfo { get; private set; }
		
		public bool IsValid { get; private set; }
		
		private RetsSession ()
		{
		}
		
		public static RetsSession Create(string baseUrl, string loginUrl, int port, string userName, string password, string userAgent, string userAgentAuth, string retsVersion)
		{
			return new RetsSession
			{
				SessionInfo = SessionInfo.Create(baseUrl, loginUrl, port, userName, password, userAgent, userAgentAuth, retsVersion)
			};
		}
		
		public RetsLoginResponse Login()
		{
			var request = new RetsRequest();
			LoginInfo = new RetsLoginResponse();

			var loginRequest = new RetsLoginRequest
				                   {
					                   MethodUrl = SessionInfo.BaseUrl + SessionInfo.LoginUrl,
					                   Session = SessionInfo
				                   };
			
			LoginInfo.Parse(request.Post(loginRequest));
			IsValid = LoginInfo.IsSuccessful;
			
			return LoginInfo;
		}
		
		public RetsGetMetadataResponse GetMetadata()
		{
			return null;
		}
		
		public RetsLogoutResponse Logout()
		{
			if (IsValid)
			{
				string transactionUrl = LoginInfo.GetCapabilityUrl(StandardCapability.Logout);
				if (!string.IsNullOrWhiteSpace(transactionUrl))
				{
					string fullLogoutUrl = transactionUrl.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || transactionUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase)
						                       ? transactionUrl
						                       : SessionInfo.BaseUrl + transactionUrl;

					var request = new RetsRequest();
					var response = new RetsLogoutResponse();

					var logoutRequest = new RetsLogoutRequest
						                    {
							                    MethodUrl = fullLogoutUrl,
							                    Session = SessionInfo
						                    };

					try
					{
						response.Parse(request.Post(logoutRequest));

						return response;
					}
					finally
					{
						LoginInfo = null;
						IsValid = false;
						SessionInfo.Cookies = null;
					}
				}
			}

			return null;
		}

		public RetsPostObjectResponse PostObject(RetsPostObjectParams postObjectParams)
		{
			if (IsValid)
			{
				string transactionUrl = LoginInfo.GetCapabilityUrl(StandardCapability.PostObject);
				if (!string.IsNullOrWhiteSpace(transactionUrl))
				{
					string fullPostObjectUrl = transactionUrl.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase) || transactionUrl.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase)
						                           ? transactionUrl
						                           : SessionInfo.BaseUrl + transactionUrl;

					var request = new RetsRequest();
					var response = new RetsPostObjectResponse();
					var postObjectRequest = new RetsPostObjectRequest
						                        {
							                        MethodUrl = fullPostObjectUrl,
							                        Session = SessionInfo,
													UpdateAction = postObjectParams.UpdateAction,
													ContentType = postObjectParams.ContentType,
													Type = postObjectParams.Type,
													Resource = postObjectParams.Resource,
													ResourceId = postObjectParams.ResourceId,
													ObjectId = postObjectParams.ObjectId,
													PostBody = postObjectParams.FileContents,
													FileName = postObjectParams.FileName,
													ContentDescription = postObjectParams.ContentDescription,
													ContentLabel = postObjectParams.ContentLabel,
													Accessibility = postObjectParams.Accessibility,
													DelegateId = postObjectParams.DelegateId,
													DelegateHash = postObjectParams.DelegateHash,
													DelegatePassword = postObjectParams.DelegatePassword
						                        };

					response.Parse(request.Post(postObjectRequest));

					return response;
				}
			}

			return null;
		}
	}
}

