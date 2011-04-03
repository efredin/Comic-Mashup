using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Fredin.Comic.Render;

namespace Fredin.Comic.Web.Models
{
	public class ClientEffect
	{
		public string Title { get; set; }
		public string ThumbUrl { get; set; }
		public ComicEffectType EffectId { get; set; }

		public ClientEffect(string title, string thumbUrl, ComicEffectType effectId)
		{
			this.Title = title;
			this.ThumbUrl = thumbUrl;
			this.EffectId = effectId;
		}
	}
}