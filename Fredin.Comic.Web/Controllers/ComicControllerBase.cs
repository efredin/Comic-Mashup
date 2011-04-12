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
	public abstract class ComicControllerBase : System.Web.Mvc.Controller
	{
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

		//#region [S3]

		///// <summary>
		///// S3 API
		///// </summary>
		//protected IThreeSharp S3 { get; set; }

		//protected virtual void InitS3()
		//{
		//    ThreeSharpConfig s3Config = new ThreeSharpConfig();
		//    s3Config.AwsAccessKeyID = ComicConfigSectionGroup.S3.PublicKey;
		//    s3Config.AwsSecretAccessKey = ComicConfigSectionGroup.S3.PrivateKey;
		//    s3Config.IsSecure = false;
		//    this.S3 = new ThreeSharpQuery(s3Config);
		//}

		//#endregion

		#region [User]

		public const string KEY_THEME = "theme";
		public const string KEY_ACTIVE_USER = "user";
		public const string KEY_FRIENDS = "friends";
		public const string KEY_GUEST_USER = "guest";

		/// <summary>
		/// Active session user. Guest users will return null
		/// </summary>
		protected virtual User ActiveUser
		{
			get { return this.Session[KEY_ACTIVE_USER] as User; }
			set { this.Session[KEY_ACTIVE_USER] = value; }
		}

		protected virtual List<long> Friends
		{
			get { return this.Session[KEY_FRIENDS] as List<long>; }
			set { this.Session[KEY_FRIENDS] = value; }
		}

		protected virtual User GuestUser
		{
			get { return this.Session[KEY_GUEST_USER] as User; }
			set { this.Session[KEY_GUEST_USER] = value; }
		}

		protected virtual string Theme
		{
			get { return this.Session[KEY_THEME] as string; }
			set { this.Session[KEY_THEME] = value; }
		}

		protected virtual void InitActiveUser()
		{
			try
			{
				if(FacebookWebContext.Current.IsAuthenticated())
				{
					if (this.ActiveUser == null || (this.ActiveUser != null && FacebookWebContext.Current.UserId != this.ActiveUser.Uid))
					{
						// Load user details into session
						this.ActiveUser = this.EntityContext.TryGetUser(FacebookWebContext.Current.UserId);
						if (this.ActiveUser == null)
						{
							// User not yet created - Create the user
							this.ActiveUser = new User();
							this.ActiveUser.Uid = FacebookWebContext.Current.UserId;

							this.EntityContext.AddToUsers(this.ActiveUser);
						}
						
						// Update from facebook - this will be modified once subscription updates are functional
						var facebookUser = (IDictionary<string, object>)this.Facebook.Get("/me");

						this.ActiveUser.Name = facebookUser["name"].ToString();
						this.ActiveUser.FbLink = facebookUser["link"].ToString();
						if(String.IsNullOrWhiteSpace(this.ActiveUser.Nickname))
						{
							this.ActiveUser.Nickname = facebookUser["name"].ToString();
						}
						if (facebookUser.ContainsKey("email"))
						{
							this.ActiveUser.Email = facebookUser["email"].ToString();
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
						
						this.Log.InfoFormat("Session created for user {0}", this.ActiveUser.Uid);
					}
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

			this.InitEntityContext();
			this.InitAzure();
			this.InitFacebook();
			this.InitActiveUser();
		}
	}
}
