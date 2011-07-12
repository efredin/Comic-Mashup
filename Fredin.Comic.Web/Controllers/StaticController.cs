using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Fredin.Comic.Web.Controllers
{
    public class StaticController : Controller
    {
		public ActionResult File(string path)
		{
			if (String.IsNullOrWhiteSpace(path))
			{
				return new HttpNotFoundResult();
			}
			else
			{
				string fullPath = Path.Combine(Server.MapPath(@"\Static"), path.Replace("/", @"\"));

				string contentType = "text/plain";
				switch (Path.GetExtension(fullPath))
				{
					case ".js":
						contentType = "application/javascript";
						break;

					case ".css":
						contentType = "text/css";
						break;

					case ".png":
						contentType = "image/png";
						break;

					case ".jpg":
						contentType = "image/jpg";
						break;

					case ".gif":
						contentType = "image/gif";
						break;
				}

				this.Response.Cache.SetCacheability(HttpCacheability.Public);
				this.Response.Cache.SetMaxAge(new TimeSpan(1, 0, 0));
				this.Response.Cache.SetExpires(DateTime.Now.AddDays(1));

				return new FilePathResult(fullPath, contentType);
			}
		}
    }
}
