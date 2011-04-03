using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Fredin.Comic.Render
{
	public class ImageRenderData
	{
		public MemoryStream SourceStream { get; set; }

		public MemoryStream RenderStream { get; set; }

		public ImageRenderData(MemoryStream sourceStream, MemoryStream renderStream)
		{
			this.SourceStream = sourceStream;
			this.RenderStream = renderStream;
		}
	}
}
