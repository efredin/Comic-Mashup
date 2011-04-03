using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fredin.Comic.Web.Facebook;

namespace Fredin.Comic.Web.Controllers
{
	public class FacebookAuthAttribute : AuthorizeAttribute
	{
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			FacebookApi facebook = new FacebookApi(httpContext);
			return facebook.HasSession;
		}

		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			// TODO: Configuration var
			filterContext.Result = new RedirectResult("~/User/Unauthorized");
		}

	}
}