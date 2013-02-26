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

using System.Xml;

namespace LpsRetsClient.Transaction
{
	public abstract class RetsResponseBase
	{
		protected const string ReplyCodeAttribute = "ReplyCode";
		protected const string ReplyTextAttribute = "ReplyText";
		protected const string RetsResponse = "RETS-RESPONSE";
		
		public XmlDocument Response { get; set; }
		
		public string ReplyCode { get; set; }
		public string ReplyText { get; set; }
		public string ResponseBody { get; set; }
		
		public bool IsSuccessful { get { return !string.IsNullOrEmpty(ReplyCode) && ReplyCode.Equals("0"); } }
		
		public virtual void Parse(string response)
		{
			if (string.IsNullOrWhiteSpace(response))
				return;

			Response = new XmlDocument();
			Response.LoadXml(response);
			
			var root = Response.DocumentElement;
			ReplyCode = root.GetAttribute(ReplyCodeAttribute);
			ReplyText = root.GetAttribute(ReplyTextAttribute);
			
			if (IsSuccessful && root.HasChildNodes)
			{
				var responseBodyNode = root.GetElementsByTagName(RetsResponse);
				if (responseBodyNode.Count > 0)
				{
					ResponseBody = responseBodyNode[0].InnerXml;
				}
			}
		}
	}
}

