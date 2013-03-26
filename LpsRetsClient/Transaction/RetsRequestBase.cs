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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using LpsRetsClient.Data;
using LpsRetsClient.Security;

namespace LpsRetsClient.Transaction
{
	public class RetsRequestBase : IRetsRequest
	{
		public string MethodUrl { get; set; }
		public SessionInfo Session { get; set; }
		public virtual byte[] PostBody { get; set; }
		public Dictionary<string, string> Headers { get; private set; }

		public string DelegateId { get; set; }
		public string DelegateHash { get; set; }
		public string DelegatePassword { get; set; }

		public RetsRequestBase()
		{
			Headers = new Dictionary<string, string>();
		}

		public virtual void PrepareRequest(HttpWebRequest request)
		{
			if (!string.IsNullOrWhiteSpace(DelegateId))
			{
				if (!request.Headers.AllKeys.Any(s => s.Equals("RETS-UA-Authorization", StringComparison.InvariantCultureIgnoreCase)))
					throw new InvalidOperationException("Missing RETS-UA-Authorization header.");

				request.Headers.Add("X-Delegate-ID", DelegateId);
				string retsUaAuthorization = request.Headers["RETS-UA-Authorization"].Substring(7);

				request.Headers.Add("X-Delegate-Authorization", GetDelegateAuthorization(retsUaAuthorization, DelegatePassword, DelegateHash, DelegateId));
			}
		}

		/// <summary>
		/// ua-digest-response ::= LHEX( MD5( RETS-UA-Authorization ":" DelegatePassword ":" DelegateHash ":" DelegateUserCode))
		/// </summary>
		/// <returns></returns>
		protected string GetDelegateAuthorization(string retsUaAuthorization, string delegatePassword, string delegateHash, string delegateId)
		{
			return "Digest " + Crypto.GetMd5(retsUaAuthorization + ":" + delegatePassword + ":" + delegateHash + ":" + delegateId);
		}
	}
}
