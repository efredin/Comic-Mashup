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
			if(url.Contains("?"))
			{
				url += String.Format("&{0}", RoleEnvironment.DeploymentId);
			}
			else
			{
				url += String.Format("?{0}", RoleEnvironment.DeploymentId);
			}

			Uri staticUrl = new Uri(new Uri(ComicConfigSectionGroup.Web.StaticBaseUrl), url);
			return staticUrl.AbsoluteUri;
		}

		public static string GetStaticUrl(string url, params object[] formatArgs)
		{
			return GetStaticUrl(String.Format(url, formatArgs));
		}

		public static string GetFacebookUrl(string url)
		{
			Uri fbUrl = new Uri(new Uri(ComicConfigSectionGroup.Web.FacebookBaseUrl), url);
			return fbUrl.AbsoluteUri;
		}

		public static string GetFacebookUrl(string url, params object[] formatArgs)
		{
			return GetFacebookUrl(String.Format(url, formatArgs));
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
			BlobConfigSection storage = ComicConfigSectionGroup.Blob;

			string directory;
			switch(render)
			{
				case RenderMode.Comic:
				default:
					directory = storage.ComicDirectory;
					break;

				case RenderMode.Frame:
					directory = storage.FrameDirectory;
					break;

				case RenderMode.Thumb:
					directory = storage.ThumbDirectory;
					break;

				case RenderMode.FrameThumb:
					directory = storage.FrameThumbDirectory;
					break;
			}

			string url = String.Format("{0}{1}", directory, comic.StorageKey);
			Uri renderUri = new Uri(new Uri(ComicConfigSectionGroup.Web.RenderBaseUrl), url);
			return renderUri.AbsoluteUri;
		}

		public static string GetPhotoUrl(Fredin.Comic.Data.Photo photo)
		{
			BlobConfigSection storage = ComicConfigSectionGroup.Blob;
			string url = String.Format("{0}{1}", storage.PhotoDirectory, photo.StorageKey);
			Uri renderUri = new Uri(new Uri(ComicConfigSectionGroup.Web.RenderBaseUrl), url);
			return renderUri.AbsoluteUri;
		}

		public static string GetProfileRenderUrl(string storageKey)
		{
			BlobConfigSection storage = ComicConfigSectionGroup.Blob;
			string url = String.Format("{0}{1}", storage.ProfileDirectory, storageKey);
			Uri renderUri = new Uri(new Uri(ComicConfigSectionGroup.Web.RenderBaseUrl), url);
			return renderUri.AbsoluteUri;
		}

		#endregion

		public static string GetReadUrl(Fredin.Comic.Data.Comic comic)
		{
			return GetWebUrl("/Comic/{0}/{1}", comic.ComicId, Urlify(comic.Title));
		}

		public static string GetAuthorUrl(Fredin.Comic.Data.User user)
		{
			return GetWebUrl("/Directory/Author/{0}/{1}", user.Uid, Urlify(user.Nickname));
		}

		public static string GetRemixUrl(Fredin.Comic.Data.Comic comic)
		{
			return GetWebUrl("/Comic/Create/{0}", comic.ComicId);
		}
	}
}