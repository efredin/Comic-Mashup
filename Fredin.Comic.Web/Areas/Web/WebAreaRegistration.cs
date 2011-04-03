using System.Web.Mvc;

namespace Fredin.Comic.Web.Areas.Web
{
	public class WebAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get { return "Web"; }
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute
			(
				"Web_default",
				"Web/{controller}/{action}/{id}",
				new { controller = "WebUser", action = "Login", id = UrlParameter.Optional }
			);
		}
	}
}
