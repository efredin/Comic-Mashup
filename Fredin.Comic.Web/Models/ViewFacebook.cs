using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Web.Models
{
	public class ViewFacebook
	{
		public List<ClientEffect> Effects { get; set; }

		public ViewFacebook(List<ClientEffect> effects)
		{
			this.Effects = effects;
		}
	}
}