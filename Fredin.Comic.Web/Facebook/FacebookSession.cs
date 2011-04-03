using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

using Fredin.Util;

namespace Fredin.Comic.Web.Facebook
{
	public class FacebookSession
	{
		#region [Property]

		protected HttpSessionStateBase Session { get; set; }

		public long Uid
		{
			get { return this.GetSessionValue<long>("uid", 0L); }
			protected set { this.SetSessionValue("uid", value); }
		}

		public string Secret
		{
			get { return this.GetSessionValue<string>("secret", null); }
			protected set { this.SetSessionValue("secret", value); }
		}

		public string AccessToken
		{
			get { return this.GetSessionValue<string>("access_token", null); }
			protected set { this.SetSessionValue("access_token", value); }
		}

		public string Signature
		{
			get { return this.GetSessionValue<string>("sig", null); }
			protected set { this.SetSessionValue("sig", value); }
		}

		public string SessionKey
		{
			get { return this.GetSessionValue<string>("session_key", null); }
			protected set { this.SetSessionValue("session_key", value); }
		}

		public DateTime Expires
		{
			get { return this.GetSessionValue<DateTime>("expires", DateTime.Now); }
			protected set { this.SetSessionValue("expires", value); }
		}

		#endregion

		public FacebookSession(HttpSessionStateBase session)
		{
			this.Session = session;
		}

		public void Load(HttpCookie cookie, string apiSecret)
		{
			this.Load(cookie.Value, apiSecret);
		}

		public void Load(string value, string apiSecret)
		{
			var args = HttpUtility.ParseQueryString(value.Replace("\"", String.Empty));
			if (!this.Validate(args, apiSecret))
			{
				throw new FacebookApiException("Invalid session signature.");
			}
			else
			{
				this.Session[this.GetSessionKey("uid")] = long.Parse(args["uid"]);
				this.Session[this.GetSessionKey("secret")] = args["secret"];
				this.Session[this.GetSessionKey("access_token")] = args["access_token"];
				this.Session[this.GetSessionKey("sig")] = args["sig"];
				this.Session[this.GetSessionKey("session_key")] = args["session_key"];
				this.Session[this.GetSessionKey("expires")] = long.Parse(args["expires"]).UnixTimeAsDateTime();
			}
		}

		public void Abandon()
		{
			this.Session.Abandon();
		}

		protected virtual void SetSessionValue(string key, object value)
		{
			this.Session[this.GetSessionKey(key)] = value;
		}

		protected virtual TValue GetSessionValue<TValue>(string key, TValue defaultValue)
		{
			object value = this.Session[this.GetSessionKey(key)];
			return value != null ? (TValue)value : defaultValue;
		}

		protected virtual string GetSessionKey(string key)
		{
			return String.Format("fbs_{0}", key);
		}

		public bool Validate(NameValueCollection args, string apiSecret)
		{
			StringBuilder payload = new StringBuilder();
			foreach (var key in args.AllKeys)
			{
				if (key != "sig")
				{
					payload.AppendFormat("{0}={1}", key, args[key]);
				}
			}
			payload.Append(apiSecret);

			return args["sig"].ToLower() == payload.ToString().ComputeMd5().ToLower();
		}
	}
}