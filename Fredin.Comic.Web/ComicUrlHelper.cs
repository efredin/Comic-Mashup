using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.ServiceRuntime;

using Fredin.Comic.Config;
using Fredin.Comic.Web.Models;
using Fredin.Comic.Render;

namespace Fredin.Comic.Web
{
	public static class ComicUrlHelper
	{
		public static string WebBaseUrl
		{
			get 
			{
				string url;
				if (HttpContext.Current.Request.ServerVariables["HTTP_HOST"] != null)
				{
					string scheme = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_PROTO"] ?? HttpContext.Current.Request.Url.Scheme;
					url = String.Format("{0}{1}{2}", scheme, Uri.SchemeDelimiter, HttpContext.Current.Request.Headers["Host"]);
				}
				else
				{
					url = new Uri(HttpContext.Current.Request.Url, HttpContext.Current.Request.RawUrl).AbsoluteUri;
				}
				return url;
			}
		}

		public static string Urlify(string text)
		{
			// Strip title down for SEO friendly, FB friendly, MVC friendly url
			text = (text.Length > 50 ? text.Substring(0, 50) : text).Replace(' ', '-'); // Limit to 50 chacters for sanity
			text = Regex.Replace(text, @"[^0-9a-zA-Z\-]", ""); // Removes all those nasty encoding characters
			return HttpUtility.UrlEncode(text); // Url encode what's left
		}

		#region [Generic Url Builders]

		public static string GetWebUrl(string url)
		{
			Uri baseUri = new Uri(WebBaseUrl);
			return new Uri(baseUri, url).AbsoluteUri;
		}

		public static string GetWebUrl(string url, params object[] formatArgs)
		{
			return GetWebUrl(String.Format(url, formatArgs));
		}

		public static string GetStaticUrl(string url)
		{
			Uri staticUrl = new Uri(new Uri(ComicConfigSectionGroup.Web.StaticBaseUrl), url);
			return staticUrl.AbsoluteUri;
		}

		public static string GetStaticUrl(string url, params object[] formatArgs)
		{
			return GetStaticUrl(String.Format(url, formatArgs));
		}

		public static string GetImageUrl(string url)
		{
			return GetStaticUrl(String.Format("Image/{0}", url));
		}

		public static string GetImageUrl(string url, params object[] formatArgs)
		{
			return GetImageUrl(String.Format(url, formatArgs));
		}

		public static string GetRenderUrl(Fredin.Comic.Data.Comic comic, RenderMode render)
		{
			StorageConfigSection storage = ComicConfigSectionGroup.Storage;

			string prefix;
			switch(render)
			{
				case RenderMode.Comic:
				default:
					prefix = storage.ComicPrefix;
					break;

				case RenderMode.Frame:
					prefix = storage.FramePrefix;
					break;

				case RenderMode.Thumb:
					prefix = storage.ThumbPrefix;
					break;

				case RenderMode.FrameThumb:
					prefix = storage.FrameThumbPrefix;
					break;
			}

			string url = String.Format("{0}{1}", prefix, comic.ComicId);
			Uri renderUri = new Uri(new Uri(ComicConfigSectionGroup.Web.RenderBaseUrl), url);
			return renderUri.AbsoluteUri;
		}

		public static string GetTaskUrl(string taskId)
		{
			Uri taskUrl = new Uri(new Uri(ComicConfigSectionGroup.Web.TaskBaseUrl), taskId);
			return taskUrl.AbsoluteUri;
		}

		#endregion

		public static string GetReadUrl(Fredin.Comic.Data.Comic comic)
		{
			return GetWebUrl("/Comic/{0}/{1}", comic.ComicId, Urlify(comic.Title));
		}

		public static string GetProfileUrl(Fredin.Comic.Data.User user)
		{
			return GetWebUrl("/User/Profile/{0}/{1}", user.Uid, Urlify(user.Nickname));
		}
	}
}