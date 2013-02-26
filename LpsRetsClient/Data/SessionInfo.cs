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
using System.Linq;
using System.Net;

namespace LpsRetsClient.Data
{
	public class SessionInfo
	{
		public string BaseUrl { get; private set; }
		public string LoginUrl { get; private set; }
		public int Port { get; private set; }
		public string UserName { get; private set; }
		public string Password { get; private set; }
		public string UserAgent { get; private set; }
		public string UserAgentAuth { get; private set; }
		public string RetsVersion { get; private set; }
		public string RetsRequestId { get; set; }
		
		public CookieContainer Cookies { get; set; }

		public string RetsSessionId 
		{ 
			get
			{
				if (Cookies != null)
				{
					foreach (Cookie cookie in Cookies.GetCookies(new Uri(BaseUrl)).Cast<Cookie>().Where(cookie => !cookie.Expired && cookie.Name.Equals("RETS-Session-ID", StringComparison.InvariantCultureIgnoreCase)))
					{
						return cookie.Value;
					}
				}

				return string.Empty;
			}
		}
		
		public string FullLoginUrl
		{
			get
			{
				if (Port > 0)
					return BaseUrl + ":" + Port + LoginUrl;
				
				return BaseUrl + LoginUrl;
			}
		}
		
		public static SessionInfo Create(string baseUrl, string loginUrl, int port, string userName, string password, string userAgent, string userAgentAuth, string retsVersion)
		{
			return new SessionInfo
			{
				BaseUrl = baseUrl,
				LoginUrl = loginUrl,
				Port = port,
				UserName = userName,
				Password = password,
				UserAgent = userAgent,
				UserAgentAuth = userAgentAuth,
				RetsVersion = retsVersion
			};
		}
	}
}

