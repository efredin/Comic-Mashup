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
		public FileResult File(string path)
		{
			string fullPath = Path.Combine(Server.MapPath(@"\Static"), path.Replace("/", @"\"));

			string contentType = String.Empty;
			switch(Path.GetExtension(fullPath))
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

			return new FilePathResult(fullPath, contentType);
		}
    }
}
