using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Facebook;
using Facebook.Web;
using Fredin.Comic.Config;
using Fredin.Comic.Data;
using Fredin.Comic.Web.Models;
using Fredin.Util;
using log4net;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Fredin.Comic.Web.Controllers
{
	[HandleError]
	public abstract class ComicControllerBase : System.Web.Mvc.Controller
	{
		protected const string VERIFY_TOKEN = "erock";
		public const string KEY_FORMAT = "f";
		public const string VAL_JSON = "json";

		/// <summary>
		/// Logger
		/// </summary>
		protected ILog Log { get; set; }

		#region [Entity]

		protected string ConnectionString { get; set; }

		private ComicModelContext _entityContext;
		public ComicModelContext EntityContext
		{
			get { return this._entityContext; }
			set
			{
				this._entityContext = value;
				this.IsEntityContextOwned = false;
			}
		}

		/// <summary>
		/// Indicates whether or not the controller owns the current context.
		/// </summary>
		protected bool IsEntityContextOwned { get; set; }

		protected virtual void InitEntityContext()
		{
			this.ConnectionString = ConfigurationManager.ConnectionStrings["ComicModelContext"].ConnectionString;

			// Attempt to find a connection string matching the current namespace
			//ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[this.GetType().Namespace];
			//if (connectionString != null)
			//{
			//    this.ConnectionString = connectionString.ConnectionString;
			//    this.Log.InfoFormat("Entity context using connection string '{0}'.", this.GetType().Namespace);
			//}
			//else
			//{
			//    throw new Exception(String.Format("Unable to find connection string '{0}' for entity context.", this.GetType().Namespace));
			//}

			this.EntityContext = new ComicModelContext(this.ConnectionString);
			this.IsEntityContextOwned = true;
		}

		#endregion

		#region [Facebook]

		protected FacebookWebClient Facebook { get; set; }

		//protected long Uid { get; set; }

		protected virtual void InitFacebook()
		{
			this.Facebook = new FacebookWebClient();

			bool fbCanvas = this.SessionManager.FbCanvas;

			// Check for forced fb removal (frame detection from js)
			if (!String.IsNullOrEmpty(this.Request[SessionHelper.KEY_FBCANVAS]))
			{
				bool.TryParse(this.Request[SessionHelper.KEY_FBCANVAS], out fbCanvas);
			}
			// Check for facebook signed post (indicates fb canvas)
			else if (FacebookWebContext.Current.SignedRequest != null)
			{
				fbCanvas = true;
			}
			this.SessionManager.FbCanvas = fbCanvas;
		}

		#endregion

		#region [Azure Services]

		protected CloudStorageAccount StorageAccount { get; set; }
		protected CloudBlobClient BlobClient { get; set; }
		protected CloudQueueClient QueueClient { get; set; }

		protected virtual void InitAzure()
		{
			this.StorageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["ComicStorage"].ConnectionString);

			this.BlobClient = this.StorageAccount.CreateCloudBlobClient();
			this.BlobClient.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.Zero);

			this.QueueClient = this.StorageAccount.CreateCloudQueueClient();
			this.QueueClient.RetryPolicy = RetryPolicies.Retry(3, TimeSpan.Zero);
		}

		#endregion

		#region [Session]

		protected SessionHelper SessionManager { get; set; }

		protected virtual void InitSessionManager()
		{
			this.SessionManager = new SessionHelper(this.HttpContext);
		}

		#endregion

		#region [User]

		/// <summary>
		/// Active session user. Guest users will return null
		/// </summary>
		protected virtual User ActiveUser
		{
			get { return this.SessionManager.ActiveUser; }
			set { this.SessionManager.ActiveUser = value; }
		}

		protected virtual List<long> Friends
		{
			get { return this.SessionManager.Friends; }
			set { this.SessionManager.Friends = value; }
		}

		protected virtual User GuestUser
		{
			get { return this.SessionManager.GuestUser; }
			set { this.SessionManager.GuestUser = value; }
		}

		protected virtual string Theme
		{
			get { return this.SessionManager.Theme; }
			set { this.SessionManager.Theme = value; }
		}

		protected virtual void InitActiveUser()
		{
			try
			{
				if (FacebookWebContext.Current.IsAuthenticated())
				{
					// Updating active user on every request now because of sync issues with deleted users
					//if (this.ActiveUser == null || (this.ActiveUser != null && FacebookWebContext.Current.UserId != this.ActiveUser.Uid))
					//{
						// Load user details into session
						this.ActiveUser = this.EntityContext.TryGetUser(FacebookWebContext.Current.UserId, true);

						// User not yet created - Create the user
						if (this.ActiveUser == null)
						{
							this.ActiveUser = new User();
							this.ActiveUser.Uid = FacebookWebContext.Current.UserId;

							this.EntityContext.AddToUsers(this.ActiveUser);
						}

						// Update from facebook - this will be modified once subscription updates are functional
						var facebookUser = (IDictionary<string, object>)this.Facebook.Get("/me");

						this.ActiveUser.IsDeleted = false; // Restored user if previously deleted (uninstall)
						this.ActiveUser.Name = facebookUser["name"].ToString();
						this.ActiveUser.FbLink = facebookUser["link"].ToString();
						this.ActiveUser.Nickname = facebookUser["name"].ToString();
						if (facebookUser.ContainsKey("email"))
						{
							this.ActiveUser.Email = facebookUser["email"].ToString();
						}
						if (facebookUser.ContainsKey("locale"))
						{
							this.ActiveUser.Locale = facebookUser["locale"].ToString().Replace('_', '-');
						}

						if (!this.ActiveUser.IsSubscribed)
						{
							this.SubscribeActiveUser();
							this.ActiveUser.IsSubscribed = true;
						}

						// Get list of friends from facebook and persist in session
						dynamic facebookFriends = this.Facebook.Get("/me/friends");
						this.Friends = new List<long>();
						foreach (dynamic f in facebookFriends.data)
						{
							this.Friends.Add(long.Parse(f.id));
						}

						this.EntityContext.SaveChanges();
						this.EntityContext.Detach(this.ActiveUser);

						// Forms authentication
						FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, this.ActiveUser.Uid.ToString(), DateTime.Now, DateTime.Now.AddMinutes(30), false, this.ActiveUser.Nickname);
						this.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(ticket)));

						this.Log.DebugFormat("Session created for user {0}", this.ActiveUser.Uid);
					//}
				}
				else
				{
					this.ActiveUser = null;
				}
			}
			catch (FacebookOAuthException authX)
			{
				this.Log.InfoFormat("Auth token invalid", authX);
				this.LogoutActiveUser();
			}
		}

		protected virtual void SubscribeActiveUser()
		{
			try
			{
				FacebookOAuthClient authClient = new FacebookOAuthClient(FacebookApplication.Current);
				dynamic auth = authClient.GetApplicationAccessToken();

				Dictionary<string, object> parameters = new Dictionary<string, object>();
				parameters.Add("object", "user");
				parameters.Add("fields", "name,link,email,locale");
				parameters.Add("callback_url", ComicUrlHelper.GetWebUrl("/User/Subscription"));
				parameters.Add("verify_token", VERIFY_TOKEN);

				FacebookClient subscriptionClient = new FacebookClient(auth.access_token);
				subscriptionClient.Post(String.Format("/{0}/subscriptions", ComicConfigSectionGroup.Facebook.AppId), parameters);
			}
			catch (Exception x)
			{
				this.Log.Error("Unable to subscribe.", x);
			}
		}

		protected virtual void LoginGuestUser(User hostUser)
		{
			this.GuestUser = hostUser;
		}

		protected virtual void LogoutActiveUser()
		{
			this.ActiveUser = null;
			this.Session.Abandon();
			FormsAuthentication.SignOut();
		}

		protected virtual bool IsFriend(User user)
		{
			bool isFriend = false;
			if (this.Friends != null)
			{
				isFriend = this.Friends.Contains(user.Uid);
			}
			else if (this.GuestUser != null)
			{
				isFriend = this.GuestUser.Uid == user.Uid;
			}
			return isFriend;
		}

		protected virtual bool IsFriendOrSelf(User user)
		{
			bool isVisible = false;
			if (this.ActiveUser != null && user.Uid == this.ActiveUser.Uid)
			{
				isVisible = true;
			}
			else
			{
				isVisible = this.IsFriend(user);
			}
			return isVisible;
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.EntityContext != null && this.IsEntityContextOwned)
			{
				this.EntityContext = null;
			}

			base.Dispose(disposing);
		}

		protected override void Initialize(System.Web.Routing.RequestContext requestContext)
		{
			base.Initialize(requestContext);

			// Initialize logger
			this.Log = LogManager.GetLogger(this.GetType());

			this.InitSessionManager();
			this.InitEntityContext();
			this.InitAzure();
			this.InitFacebook();
			this.InitActiveUser();

			this.Localize();
		}

		private void Localize()
		{
			if (this.SessionManager.Locale == null)
			{
				if (this.ActiveUser != null && !String.IsNullOrWhiteSpace(this.ActiveUser.Locale))
				{
					this.SessionManager.Locale = this.ActiveUser.Locale;
				}
				else if (this.GuestUser != null && !String.IsNullOrWhiteSpace(this.GuestUser.Locale))
				{
					this.SessionManager.Locale = this.GuestUser.Locale;
				}
			}
		}

		protected override void OnActionExecuted(ActionExecutedContext filterContext)
		{
			var action = filterContext.Result as ViewResult;
			if (action != null)
			{
				action.MasterName = this.SessionManager.FbCanvas ? "Facebook" : "Web";
			}

			base.OnActionExecuted(filterContext);
		}
	}
}
