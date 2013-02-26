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

namespace LpsRetsClient
{
	public class RetsPostObjectParams
	{
		public string UpdateAction { get; set; }
		public string ContentType { get; set; }
		public string Type { get; set; }
		public string Resource { get; set; }
		public string ResourceId { get; set; }
		public string Accessibility { get; set; }
		public string ContentLabel { get; set; }
		public string ContentDescription { get; set; }
		public string FileName { get; set; }
		public int? ObjectId { get; set; }

		public byte[] FileContents { get; set; }

		public static RetsPostObjectParams ActionAdd(string contentType, string type, string resource, string resourceId, int? objectId, string fileName, byte[] fileContents, string description = null, string label = null, string accessibility = null)
		{
			return new RetsPostObjectParams
				       {
					       UpdateAction = "Add",
					       ContentType = contentType,
					       Type = type,
					       Resource = resource,
					       ResourceId = resourceId,
					       ObjectId = objectId,
						   FileContents = fileContents,
						   Accessibility = accessibility,
						   ContentLabel = label,
						   ContentDescription = description,
						   FileName = fileName
				       };
		}

		public static RetsPostObjectParams ActionReplace(string contentType, string type, string resource, string resourceId, int objectId, string fileName, byte[] fileContents, string description = null, string label = null, string accessibility = null)
		{
			return new RetsPostObjectParams
			{
				UpdateAction = "Replace",
				ContentType = contentType,
				Type = type,
				Resource = resource,
				ResourceId = resourceId,
				ObjectId = objectId,
				FileContents = fileContents,
				Accessibility = accessibility,
				ContentLabel = label,
				ContentDescription = description,
				FileName = fileName
			};
		}

		public static RetsPostObjectParams ActionDelete(string contentType, string type, string resource, string resourceId, int? objectId)
		{
			return new RetsPostObjectParams
			{
				UpdateAction = "Delete",
				ContentType = contentType,
				Type = type,
				Resource = resource,
				ResourceId = resourceId,
				ObjectId = objectId
			};
		}
	}
}
