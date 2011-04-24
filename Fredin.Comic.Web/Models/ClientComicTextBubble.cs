using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ClientComicTextBubble
	{
		public long ComicTextBubbleId { get; set; }
		public ClientTextBubbleDirection TextBubbleDirection { get; set; }
		public string Text { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public ClientComicTextBubble(ComicTextBubble source)
		{
			this.ComicTextBubbleId = source.ComicTextBubbleId;
			this.TextBubbleDirection = new ClientTextBubbleDirection(source.TextBubbleDirection);
			this.Text = source.Text;
			this.X = source.X;
			this.Y = source.Y;
		}
	}
}