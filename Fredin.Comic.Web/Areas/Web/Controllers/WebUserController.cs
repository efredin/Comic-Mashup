using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Fredin.Comic.Web.Areas.Web.Models;
using Fredin.Comic.Web.Controllers;
using Fredin.Comic.Web.Models;
using Fredin.Util;

namespace Fredin.Comic.Web.Areas.Web.Controllers
{
    public class WebUserController : UserController
    {
		private const string KEY_REQUEST_URL = "url";
		protected virtual string RequestUrl
		{
			get { return this.Session[KEY_REQUEST_URL] as String; }
			set { this.Session[KEY_REQUEST_URL] = value; }
		}

		public ActionResult Login()
		{
			return this.View();
		}

		[HttpPost]
		public ActionResult Login(LoginModel model)
		{
			string passwordHash = model.Password.ComputeMd5();
			EmailUser user = this.EntityContext.TryGetEmailUser(model.Email);
			if (user != null && user.PasswordHash == passwordHash)
			{
				if (!user.IsValidated)
				{
					// Redirect to validation page
					this.Server.Transfer("~/User/Validate");
				}

				// Redirect to requested url
				if (!String.IsNullOrWhiteSpace(this.RequestUrl))
				{
					this.Server.Transfer(this.RequestUrl);
				}
				else
				{
					this.Server.Transfer("~/");
				}
			}

			return this.View(model);
		}
    }
}
