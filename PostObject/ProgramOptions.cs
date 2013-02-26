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

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Args;

namespace PostObject
{
	[Description("Use PostObject transaction against a RETS server")]
	public class ProgramOptions
	{
		[Required, ArgsMemberSwitch("t"), Description("The object type to post (i.e., Document or Photo)")]
		public string Type { get; set; }

		[Required, ArgsMemberSwitch("r"), Description("The resource for the object (i.e., Property)")]
		public string Resource { get; set; }

		[Required, ArgsMemberSwitch("i"), Description("The ID of the entity within the resource (i.e., listing ID)")]
		public string ResourceId { get; set; }

		[ArgsMemberSwitch("o"), Description("The object index to add, replace, or delete")]
		public int? ObjectId { get; set; }

		[Required, ArgsMemberSwitch("c"), Description("The content type of the object (i.e., image/jpeg)"), DefaultValue("image/jpeg")]
		public string ContentType { get; set; }

		[ArgsMemberSwitch("d"), Description("The description to apply to the object (if supported)")]
		public string Description { get; set; }

		[ArgsMemberSwitch("l"), Description("The label to apply to the object (if supported)")]
		public string Label { get; set; }

		[ArgsMemberSwitch("acc"), Description("The accessibility to apply to the object (if supported)")]
		public string Accessibility { get; set; }

		[Required, ArgsMemberSwitch("u"), Description("The user name to use for login")]
		public string UserName { get; set; }

		[Required, ArgsMemberSwitch("url"), Description("The login URL for the RETS server")]
		public string LoginUrl { get; set; }

		[ArgsMemberSwitch("p"), Description("The port of the RETS server"), DefaultValue(80)]
		public int Port { get; set; }

		[Required, ArgsMemberSwitch(0), Description("The action to perform (Add, Replace, Delete)")]
		public string Action { get; set; }

		[ArgsMemberSwitch("f"), Description("The file to add or replace")]
		public string File { get; set; }
		
		[Required, ArgsMemberSwitch("ua"), Description("The user agent to use for the RETS transactions"), DefaultValue("LpsPostObject/1.0")]
		public string UserAgent { get; set; }

		[ArgsMemberSwitch("uap"), Description("The user agent password for this user agent, if applicable")]
		public string UserAgentAuth { get; set; }

		[Required, ArgsMemberSwitch("v"), Description("The RETS version to use"), DefaultValue("RETS/1.5")]
		public string RetsVersion { get; set; }

		[ArgsMemberSwitch("verbose"), Description("Set for more verbose output.")]
		public bool Verbose { get; set; }
	}
}
