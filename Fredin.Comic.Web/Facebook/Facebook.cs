using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Fredin.Comic.Web.Config;
using System.Net;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;

namespace Fredin.Comic.Web.Facebook
{
	public class FacebookApi
	{
		public const string URL_GRAPH = "http://graph.facebook.com/";
		public const string URL_GRAPH_SECURE = "https://graph.facebook.com/";

		protected virtual FacebookConfigSection Config { get; set; }

		protected virtual HttpContextBase Context { get; set; }

		public virtual FacebookSession Session { get; protected set; }

		public bool HasSession
		{
			get { return this.Session.Uid > 0L; }
		}

		public FacebookApi() : this(new HttpContextWrapper(HttpContext.Current))
		{
		}

		public FacebookApi(HttpContextBase context)
		{
			this.Context = context;
			this.Config = ComicConfigSectionGroup.Facebook;
			this.Session = new FacebookSession(this.Context.Session);

			if (!this.HasSession)
			{
				// Attempt to load the session from cookies
				HttpCookie sessionCookie = this.Context.Request.Cookies[String.Format("fbs_{0}", this.Config.ApiKey)];
				if (sessionCookie != null)
				{
					this.Session.Load(sessionCookie, this.Config.ApiSecret);
				}
			}
		}

		public dynamic GetGraph(string path, string httpMethod = "GET", bool requireSession = true)
		{
			return this.GetGraph(path, null, httpMethod, requireSession);
		}

		public dynamic GetGraph(string path, dynamic parameters, string httpMethod = "GET", bool requireSession = true)
		{
			if (parameters == null)
			{
				parameters = new JsonObject();
			}

			string url;
			if (requireSession)
			{
				if (!this.HasSession)
				{
					throw new FacebookApiException("No session found.");
				}
				parameters.access_token = this.Session.AccessToken;
				url = String.Concat(URL_GRAPH_SECURE, path);
			}
			else
			{
				url = string.Concat(URL_GRAPH, path);
			}

			dynamic client = new RestClient(url);

			try
			{
				return client(parameters, httpMethod);
			}
			catch (WebException webX)
			{
				if (webX.Response != null)
				{
					dynamic response = null;
					try
					{
						StreamReader xReader = new StreamReader(webX.Response.GetResponseStream());
						JsonReader jsonReader = new JsonReader(xReader.ReadToEnd());
						response = jsonReader.ReadValue();
					}
					catch
					{
					}

					if (response != null)
					{
						try
						{
							throw new FacebookApiException(string.Format("{0}: {1}", response.error.type, response.error.message));
						}
						catch (RuntimeBinderException)
						{
						}
					}
				}
				throw webX;
			}
		}
	}
}