using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ClientTextBubbleDirection
	{
		public long TextBubbleDirectionId { get; set; }
		public string Title { get; set; }
		public int BaseScaleX { get; set; }
		public int BaseScaleY { get; set; }
		public int TextScaleX { get; set; }
		public int TextScaleY { get; set; }
		public string Direction { get; set; }

		public string ImageUrl { get; set; }

		public ClientTextBubbleDirection(TextBubbleDirection direction)
		{
			this.TextBubbleDirectionId = direction.TextBubbleDirectionId;
			this.Title = direction.TextBubble.Title;
			this.BaseScaleX = direction.TextBubble.BaseScaleX;
			this.BaseScaleY = direction.TextBubble.BaseScaleY;
			this.TextScaleX = direction.TextBubble.TextScaleX;
			this.TextScaleY = direction.TextBubble.TextScaleY;
			this.Direction = direction.Direction;

			this.ImageUrl = ComicUrlHelper.GetStaticUrl("Image/TextBubble/{0}-{1}.png", direction.TextBubble.Title, direction.Direction);
		}
	}
}