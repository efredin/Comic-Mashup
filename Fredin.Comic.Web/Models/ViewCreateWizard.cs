using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Web.Models
{
	public class ViewCreateWizard
	{
		public List<ClientTemplate> Templates { get; set; }
		public List<ClientEffect> Effects { get; set; }

		public ViewCreateWizard(List<ClientTemplate> templates, List<ClientEffect> effects)
		{
			this.Templates = templates;
			this.Effects = effects;
		}
	}
}