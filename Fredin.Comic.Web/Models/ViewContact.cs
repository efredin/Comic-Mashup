using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Web.Models
{
	public class ViewContact
	{
		public string Nickname { get; set; }
		public string Email { get; set; }
		public string Message { get; set; }

		public ViewContact()
		{
		}
	}
}