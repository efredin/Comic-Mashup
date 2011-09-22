using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Fredin.Comic.Web.Models
{
	public class ViewSettings
	{
		public string Feedback { get; set; }

		public ClientUserEngage Engage { get; set; }

		[Email]
		public string Email { get; set; }

		public ViewSettings()
		{
			this.Engage = new ClientUserEngage();
		}

		public ViewSettings(ClientUserEngage engage, string email)
		{
			this.Engage = engage;
			this.Email = email;
		}
	}
}