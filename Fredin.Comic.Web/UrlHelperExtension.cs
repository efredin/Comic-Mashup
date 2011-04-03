using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Fredin.Comic.Config;
using Fredin.Comic.Web.Models;
using Fredin.Comic.Render;

namespace Fredin.Comic.Web
{
	public static class UrlHelperExtension
	{
		public static string Static(this UrlHelper helper, string url)
		{
			return ComicUrlHelper.GetStaticUrl(url);
		}

		public static string Static(this UrlHelper helper, string url, params object[] formatArgs)
		{
			return ComicUrlHelper.GetStaticUrl(url, formatArgs);
		}

		public static string Render(this UrlHelper helper, Fredin.Comic.Data.Comic comic, RenderMode render)
		{
			return ComicUrlHelper.GetRenderUrl(comic, render);
		}
	}
}