using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Web.Models
{
	public class ViewCreate
	{
		public List<ClientTemplate> Templates { get; set; }
		public List<ClientEffect> Effects { get; set; }
		public List<ClientTextBubbleDirection> Bubbles { get; set; }

		public ViewCreate(List<ClientTemplate> templates, List<ClientEffect> effects)
		{
			this.Templates = templates;
			this.Effects = effects;
		}

		public ViewCreate(List<ClientTemplate> templates, List<ClientEffect> effects, List<ClientTextBubbleDirection> bubbles)
		{
			this.Templates = templates;
			this.Effects = effects;
			this.Bubbles = bubbles;
		}
	}
}