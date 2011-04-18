using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using log4net;
using Fredin.Comic.Data;
using Fredin.Comic.Web;
using Facebook;
using Facebook.Web;
using Fredin.Comic.Config;

namespace Fredin.Comic.Worker
{
	public class UserSubscriber
	{
		#region [Singelton]
		private static UserSubscriber _instance;
		public static UserSubscriber Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new UserSubscriber();
				}
				return _instance;
			}
		}
		static UserSubscriber()
		{
			Log = LogManager.GetLogger(typeof(UserSubscriber));
		}
		#endregion

		private static ILog Log { get; set; }
		private System.Threading.Timer UpdateTimer { get; set; }

		private UserSubscriber()
		{
		}

		public void Start()
		{
			Log.Info("Starting");
			this.UpdateTimer = new System.Threading.Timer(delegate(object state)
			{
				try
				{
					FacebookOAuthClient authClient = new FacebookOAuthClient(FacebookApplication.Current);
					string token = authClient.GetApplicationAccessToken() as string;

					FacebookClient facebook = new FacebookClient(token);
					IFacebookApplication app = ConfigurationManager.GetSection("facebookSettings") as IFacebookApplication;

					Dictionary<string, object> authParameters = new Dictionary<string, object>();
					authParameters.Add("client_id", app.AppId);
					authParameters.Add("client_secret", app.AppSecret);
					authParameters.Add("grant_type", "client_credentials");

					facebook.Get("/oauth/access_token", authParameters);


					ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["ComicModelContext"];
					using (ComicModelContext context = new ComicModelContext(connectionString.ConnectionString))
					{
						foreach (User user in context.ListUnsubscriberUsers())
						{
							Dictionary<string, object> parameters = new Dictionary<string, object>();
							parameters.Add("object", "user");
							parameters.Add("fields", "name,link,email,locale");
							parameters.Add("callback_url", ComicUrlHelper.GetWebUrl("/User/Subscription"));
							parameters.Add("verify_token", "erock");


							//this.Facebook.Post(String.Format("/{0}/subscriptions", ComicConfigSectionGroup.Facebook.AppId), parameters);
							//this.ActiveUser.IsSubscribed = true;
						}
					}
				}
				catch (Exception x)
				{
					Log.Error(x);
				}

			}, null, new TimeSpan(0, 0, 0), new TimeSpan(1, 0, 0));
		}

		public void Stop()
		{
			Log.Info("Stopping");

			if (this.UpdateTimer != null)
			{
				this.UpdateTimer.Dispose();
				this.UpdateTimer = null;
			}
		}
	}
}