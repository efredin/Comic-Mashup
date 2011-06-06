using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Web.Models
{
	public class ViewCreate
	{
		public ClientComic Comic { get; set; }
		public List<ClientTemplate> Templates { get; set; }
		public List<ClientEffect> Effects { get; set; }
		public List<ClientTextBubbleDirection> Bubbles { get; set; }

		public ViewCreate(ClientComic comic, List<ClientTemplate> templates, List<ClientEffect> effects)
		{
			this.Comic = comic;
			this.Templates = templates;
			this.Effects = effects;
		}

		public ViewCreate(ClientComic comic, List<ClientTemplate> templates, List<ClientEffect> effects, List<ClientTextBubbleDirection> bubbles)
		{
			this.Comic = comic;
			this.Templates = templates;
			this.Effects = effects;
			this.Bubbles = bubbles;
		}
	}
}