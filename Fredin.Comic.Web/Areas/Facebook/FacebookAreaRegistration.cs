using System.Web.Mvc;

namespace Fredin.Comic.Web.Areas.Facebook
{
	public class FacebookAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "Facebook";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
				"Facebook_default",
				"Facebook/{controller}/{action}/{id}",
				new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
