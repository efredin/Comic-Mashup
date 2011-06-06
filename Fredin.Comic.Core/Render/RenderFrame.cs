using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fredin.Comic.Render
{
	public class RenderFrame
	{
		public long Id { get; set; }
		public string Message { get; set; }
		public string PhotoUrl { get; set; }
		public long? PhotoId { get; set; }
	}
}
