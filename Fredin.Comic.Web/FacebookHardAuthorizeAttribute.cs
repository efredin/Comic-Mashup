using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Facebook.Web;
using Facebook.Web.Mvc;
using System.Web.Mvc;
using Facebook;

namespace Fredin.Comic.Web
{
	public class FacebookHardAuthorizeAttribute : FacebookAuthorizeAttributeBase
	{
		public override void OnAuthorization(AuthorizationContext filterContext, IFacebookApplication facebookApplication)
		{
			var authorizer = new FacebookWebContext(facebookApplication, filterContext.HttpContext);

			if (!authorizer.IsAuthorized(string.IsNullOrEmpty(Permissions) ? null : Permissions.Split(',')))
			{
				throw new UnauthorizedAccessException();
			}
		}
	}
}