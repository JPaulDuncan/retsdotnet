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
using LpsRetsClient.Data;

namespace LpsRetsClient.Transaction
{
	public class RetsLoginResponse : RetsResponseBase
	{
		public string MemberName { get; set; }
		public User User { get; set; }
		public Broker Broker { get; set; }
		public MetadataVersion MetadataVersion { get; set; }
		public MetadataVersion MinMetadataVersion { get; set; }
		public string MetadataTimestamp { get; set; }
		public string MinMetadataTimestamp { get; set; }
		public string OfficeList { get; set; }
		public string Balance { get; set; }
		public string TimeoutSeconds { get; set; }
		public string Expr { get; set; }
		
		public Dictionary<string, string> CapabilityUrl { get; private set; }
		
		public RetsLoginResponse ()
		{
			CapabilityUrl = new Dictionary<string, string>();
		}
		
		public override void Parse(string response)
		{
			base.Parse(response);
			
			if (!IsSuccessful || string.IsNullOrEmpty(ResponseBody))
				return;
			
			var attributes = ResponseBody.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (var attribute in attributes)
			{
				int separatorIndex = attribute.IndexOf('=');
				if (separatorIndex < 1)
					continue;
				
				var key = attribute.Substring(0, separatorIndex).Trim();
				var value = attribute.Substring(separatorIndex + 1).Trim();
				
				switch (key.ToLower())
				{
					case "membername":
						MemberName = value;
						break;
					
					case "user":
						User = User.Parse(value);
						break;
				
					case "broker":
						Broker = Broker.Parse(value);
						break;
					
					case "metadataversion":
						MetadataVersion = MetadataVersion.Parse(value);
						break;
					
					case "minmetadataversion":
						MinMetadataVersion = MetadataVersion.Parse(value);
						break;
					
					case "metadatatimestamp":
						break;
						
					case "minmetadatatimestamp":
						break;
						
					case "timeoutseconds":
						break;
						
					case "balance":
						break;
						
					case "expr":
						break;
						
					case "officelist":
						break;
						
					default:
						try
						{
							var capability = (StandardCapability)Enum.Parse(typeof(StandardCapability), value, true);
							CapabilityUrl[capability.ToString()] = value;
						}
						catch
						{
							// Custom URL
							CapabilityUrl[key] = value;
						}
						
						break;
				}
			}
		}

		public string GetCapabilityUrl(StandardCapability capability)
		{
			return GetCapabilityUrl(capability.ToString());
		}

		public string GetCapabilityUrl(string capability)
		{
			string transactionUrl;
			CapabilityUrl.TryGetValue(capability, out transactionUrl);

			return transactionUrl;
		}
	}
}

