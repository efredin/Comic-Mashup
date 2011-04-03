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
		public virtual User ActiveUser
		{
			get { return this.Session[ComicControllerBase.KEY_ACTIVE_USER] as User; }
		}

		protected virtual string Theme
		{
			get
			{
				string theme = this.Session[ComicControllerBase.KEY_THEME] as String;
				if (String.IsNullOrWhiteSpace(theme))
				{
					theme = "mashup";
				}
				return theme;
			}
		}

		public string Json(object data)
		{
			return new JavaScriptSerializer().Serialize(data);
		}
	}
}