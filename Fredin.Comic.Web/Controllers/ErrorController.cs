using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

using Fredin.Comic.Web.Models;
using Fredin.Comic.Data;
using Fredin.Util;
using Facebook.Web.Mvc;
using System.Web.Script.Serialization;
using Facebook;
using Facebook.Web;

namespace Fredin.Comic.Web.Controllers
{
	public class ErrorController : ComicControllerBase
	{
		[JsonAction]
		public EmptyResult LogError(string x)
		{
			this.Log.ErrorFormat("Client error. {0}", x);
			return new EmptyResult();
		}
	}
}