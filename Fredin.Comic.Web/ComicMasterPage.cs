using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Fredin.Comic.Web.Models;
using Fredin.Comic.Web.Controllers;
using System.Web.Script.Serialization;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web
{
	public class ComicMasterPage : System.Web.Mvc.ViewMasterPage
	{
		protected SessionHelper SessionManager { get; set; }

		protected override void OnInit(EventArgs e)
		{
			this.SessionManager = new SessionHelper(this.ViewContext.HttpContext);
			base.OnInit(e);
		}

		public virtual User ActiveUser
		{
			get { return this.SessionManager.ActiveUser; }
		}

		protected virtual string Theme
		{
			get { return this.SessionManager.Theme; }
		}

		public string Json(object data)
		{
			return new JavaScriptSerializer().Serialize(data);
		}
	}
}