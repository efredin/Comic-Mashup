using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fredin.Comic.Render
{
	[Serializable]
	public class RenderBubble
	{
		public long TextBubbleDirectionId { get; set; }
		public string Text { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
	}
}
