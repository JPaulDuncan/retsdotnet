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

namespace LpsRetsClient.Data
{
	public class MetadataVersion
	{
		public byte Major { get; set; }
		public byte Minor { get; set; }
		public int? Release { get; set; }
		
		public static MetadataVersion Parse(string metadata)
		{
			var metadataVersion = new MetadataVersion();
			
			if (!string.IsNullOrEmpty(metadata))
			{
				var versionComponents = metadata.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				if (versionComponents.Length > 0)
				{
					byte value = 0;
					if (byte.TryParse(versionComponents[0], out value))
					{
						metadataVersion.Major = value;
						
						if (versionComponents.Length > 1 && byte.TryParse(versionComponents[1], out value))
						{
							metadataVersion.Minor = value;
							
							if (versionComponents.Length > 2)
							{
								int release = 0;
								if (int.TryParse(versionComponents[2], out release))
								{
									metadataVersion.Release = release;
								}
							}
						}
					}
				}
			}
			
			return metadataVersion;
		}
		
		public override string ToString ()
		{
			 return string.Format("{0}.{1}{2}{3}", Major, Minor, Release.HasValue ? "." : string.Empty, Release);
		}
	}
}

