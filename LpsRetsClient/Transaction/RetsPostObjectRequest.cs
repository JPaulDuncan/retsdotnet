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

namespace LpsRetsClient.Transaction
{
	public class RetsPostObjectRequest : RetsRequestBase
	{
		public string UpdateAction { get; set; }
		public string ContentType { get; set; }
		public int ContentLength { get; set; }
		public string Type { get; set; }
		public string Resource { get; set; }
		public string ResourceId { get; set; }
		public int? ObjectId { get; set; }

		public string Accessibility { get; set; }
		public string ContentDescription { get; set; }
		public string ContentLabel { get; set; }
		public string FileName { get; set; }
		public int? DocumentTypeId { get; set; }

		public override void PrepareRequest(HttpWebRequest request)
		{
			request.ContentType = ContentType;

			request.Headers.Add("UpdateAction", UpdateAction);
			request.Headers.Add("Type", Type);
			request.Headers.Add("Resource", Resource);
			request.Headers.Add("ResourceID", ResourceId);

			if (!string.IsNullOrWhiteSpace(Accessibility))
				request.Headers.Add("X-Accessibility", Accessibility);

			if (!string.IsNullOrWhiteSpace(ContentDescription))
				request.Headers.Add("X-Content-Description", ContentDescription);

			if (!string.IsNullOrWhiteSpace(ContentLabel))
				request.Headers.Add("X-Content-Label", ContentLabel);

			if (ObjectId.HasValue)
				request.Headers.Add("ObjectID", ObjectId.ToString());

			if (Type.Equals("Document", StringComparison.InvariantCultureIgnoreCase))
				request.Headers.Add("X-FileName", FileName);

			if (Type.Equals("Document", StringComparison.InvariantCultureIgnoreCase) && DocumentTypeId.HasValue)
				request.Headers.Add("X-Document-Type-ID", DocumentTypeId.ToString());

			base.PrepareRequest(request);
		}
	}
}
