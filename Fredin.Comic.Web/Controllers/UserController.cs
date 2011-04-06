using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

using Fredin.Comic.Web.Models;
using Fredin.Comic.Data;
using Fredin.Util;
using Facebook.Web.Mvc;

namespace Fredin.Comic.Web.Controllers
{
	[HandleError]
	[JsonAction]
	public class UserController : ComicControllerBase
	{
		public ViewResult Login()
		{
			return this.View();
		}

		public EmptyResult Logout()
		{
			this.LogoutActiveUser();
			return null;
		}

		public JsonResult Me()
		{
			ClientUser user = null;
			if(this.ActiveUser != null)
			{
				user = new ClientUser(this.ActiveUser);
			}

			return this.Json(user, JsonRequestBehavior.AllowGet);
		}

		public ActionResult Profile(long? uid, string nickname)
		{
			ActionResult result;

			User user = null;

			try
			{
				this.EntityContext.TryAttach(this.ActiveUser);

				if(!uid.HasValue && this.ActiveUser != null)
				{
					user = this.ActiveUser;
				}
				else if (uid.HasValue)
				{
					user = this.EntityContext.TryGetUser(uid.Value);
					this.EntityContext.TryAttach(user);
				}

				if (user != null)
				{
					// Load published comics
					List<Data.Comic> comics = this.EntityContext.ListPublishedComics(user, this.ActiveUser, this.IsFriend(user), ComicStat.ComicStatPeriod.AllTime)
						.OrderByDescending(c => c.PublishTime)
						.ToList();

					// Get stats for each comic
					if(this.Request[KEY_FORMAT] == VAL_JSON)
					{
						result = this.Json(new ViewProfile(user, comics), JsonRequestBehavior.AllowGet);
					}
					else
					{
						result = this.View(new ViewProfile(user, comics));
					}
				}
				else
				{
					if (uid.HasValue)
					{
						throw new Exception("Unknown user.");
					}
					else
					{
						result = this.RedirectToAction("Login");
					}
				}
			}
			finally
			{
				this.EntityContext.TryDetach(this.ActiveUser);
				this.EntityContext.TryDetach(user);
			}

			return result;
		}

		public JsonResult ChangeTheme(string theme)
		{
			this.Theme = theme;
			return this.Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
		}

		//public bool Login(string email, string password)
		//{
		//    bool loginSuccess = false;
		//    string hash = password.ComputeMd5();

		//    return this.EntityContext.User.OfType<EmailUser>()
		//        .FirstOrDefault(e => e.Email == email && e.PasswordHash == hash);


		//    using (UserController controller = new UserController(this.EntityContext))
		//    {
		//        EmailUser user = controller.TryLoginEmailUser(email, password);

		//        if (user != null)
		//        {
		//            this.ActiveUser = new ClientEmailUser(user);
		//            if (!user.IsValidated)
		//            {
		//                // May cause Server.Transfer
		//                this.RedirectValidate();
		//            }
		//            else
		//            {
		//                loginSuccess = true;
		//            }
		//        }
		//    }

		//    return loginSuccess;
		//}
	}
}