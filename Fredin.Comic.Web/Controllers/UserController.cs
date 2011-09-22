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
using System.Web.Script.Serialization;
using Facebook;
using Facebook.Web;

namespace Fredin.Comic.Web.Controllers
{
	public class UserController : ComicControllerBase
	{
		[JsonAction]
		public ViewResult Login()
		{
			return this.View();
		}

		[JsonAction]
		public EmptyResult Logout()
		{
			this.LogoutActiveUser();
			return null;
		}

		[JsonAction]
		public JsonResult Me()
		{
			ClientUser user = null;
			if(this.ActiveUser != null)
			{
				user = new ClientUser(this.ActiveUser);
			}

			return this.Json(user, JsonRequestBehavior.AllowGet);
		}

		[JsonAction]
		public ActionResult Profile(long? id, string title)
		{
			return this.RedirectToAction("Author", "Directory", new { uid = id, nickname = title } );
		}

		[HttpGet]
		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		[JsonAction]
		public ActionResult Settings()
		{
			ActionResult result = new EmptyResult();

			try
			{
				this.EntityContext.TryAttach(this.ActiveUser);
				UserEngage engage = this.GetUserEngage(this.ActiveUser);
				result = this.View(new ViewSettings(new ClientUserEngage(engage), this.ActiveUser.Email));
			}
			finally
			{
				this.EntityContext.TryDetach(this.ActiveUser);
			}

			return result;
		}

		[HttpPost]
		[FacebookAuthorize(LoginUrl = "~/User/Login")]
		public ActionResult Settings(ViewSettings settings)
		{
			ActionResult result = new EmptyResult();
			try
			{
				this.EntityContext.TryAttach(this.ActiveUser);
				UserEngage engage = this.GetUserEngage(this.ActiveUser);

				this.ActiveUser.Email = settings.Email;
				engage.Unsubscribe = settings.Engage.Unsubscribe;
				engage.Comment = settings.Engage.Comment;
				engage.ComicCreate = settings.Engage.ComicCreate;
				engage.ComicRemix = settings.Engage.ComicRemix;

				this.EntityContext.SaveChanges();

				ViewSettings model = new ViewSettings(new ClientUserEngage(engage), this.ActiveUser.Email);
				model.Feedback = "Your changes have been saved.";

				result = this.View(model);
			}
			finally
			{
				this.EntityContext.TryDetach(this.ActiveUser);
			}
			return result;
		}

		[HttpGet]
		public ActionResult Unsubscribe(long id, string emailHash)
		{
			UserEngageHistory history = this.EntityContext.TryGetUserEngageHistory(id);
			if (history == null)
			{
				throw new Exception(String.Format("Unable to unsubscribe. Could not find the requested engagement {0}", id));
			}

			history.UserReference.Load();
			history.User.UserEngageReference.Load();

			if (history.User.Email.ComputeMd5() != emailHash)
			{
				throw new Exception(String.Format("Unable to verify hash for engagement {0}. Received hash {1}", id, emailHash));
			}

			history.RespondTime = DateTime.Now;
			history.User.UserEngage.Unsubscribe = true;
			this.EntityContext.SaveChanges();

			return this.View();
		}

		/// <summary>
		/// Facebook uninstall callback
		/// </summary>
		[HttpPost]
		public EmptyResult Uninstall()
		{
			FacebookSignedRequest request = FacebookWebContext.Current.SignedRequest;
			if(request == null)
			{
				throw new Exception("Invalid request.");
			}

			User user = this.EntityContext.TryGetUser(request.UserId, true);
			if(user != null)
			{
				user.IsDeleted = true;
				user.IsSubscribed = false;
				this.EntityContext.SaveChanges();
			}
	
			return new EmptyResult();
		}

		/// <summary>
		/// Facebook subscription verificication handler
		/// </summary>
		[HttpGet]
		[FacebookSubscriptionVerify(VERIFY_TOKEN)]
		public FacebookSubscriptionVerifiedResult Subscription()
		{
			return new FacebookSubscriptionVerifiedResult();
		}

		/// <summary>
		/// Facebook subscription callback handler
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		[HttpPost]
		[FacebookSubscriptionReceived(ParameterName = "data")]
		public EmptyResult Subscription(Dictionary<string, object> data)
		{
			if (data != null) // Facebook passing empty json arguments ?
			{
				if (data["object"].ToString() == "user")
				{
					foreach (Dictionary<string, object> u in ((Dictionary<string, object>)data["entry"]).Values)
					{
						try
						{
							long uid = long.Parse(u["uid"].ToString());
							User user = this.EntityContext.TryGetUser(uid);
							if (user != null)
							{
								if (u.ContainsKey("name"))
								{
									user.Name = u["name"].ToString();
									user.Nickname = user.Name;
								}
								if (u.ContainsKey("locale"))
								{
									user.Locale = u["locale"].ToString();
								}
								if (u.ContainsKey("link"))
								{
									user.FbLink = u["link"].ToString();
								}
								if (u.ContainsKey("email"))
								{
									user.Email = u["email"].ToString();
								}
								user.IsSubscribed = true; // Just in case
								this.EntityContext.SaveChanges();
							}
						}
						catch (Exception x)
						{
							this.Log.Error("Unable to handle subscription callback.", x);
						}
					}
				}
				else if (data["object"].ToString() == "permissions")
				{
					// Not implemented
					throw new NotImplementedException("Cannot handle permissions subscription callbacks.");
				}
			}

			return new EmptyResult();
		}

		[JsonAction]
		public JsonResult ChangeTheme(string theme)
		{
			this.Theme = theme;
			return this.Json(new { result = "ok" }, JsonRequestBehavior.AllowGet);
		}
	}
}