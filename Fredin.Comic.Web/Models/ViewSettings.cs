using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Web.Models
{
	public class ViewSettings
	{
		public ClientUserEngage Engage { get; set; }
		public string Email { get; set; }

		public ViewSettings(ClientUserEngage engage, string email)
		{
			this.Engage = engage;
			this.Email = email;
		}
	}
}