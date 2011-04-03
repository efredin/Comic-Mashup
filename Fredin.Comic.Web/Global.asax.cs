using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using System.Web.Mvc;
using log4net.Config;
using log4net.Appender;
using Microsoft.WindowsAzure.StorageClient;

namespace Fredin.Comic.Web
{
	public class Global : System.Web.HttpApplication
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute
			(
				"Read",
				"Comic/{comicId}/{*title}",
				new { controller = "Comic", action = "Read", title = UrlParameter.Optional },
				new { comicId = @"\d+" }
			);

			routes.MapRoute
			(
				"Profile",
				"User/Profile/{uid}/{*nickname}",
				new { controller = "User", action = "Profile", uid = UrlParameter.Optional, nickname = UrlParameter.Optional }
			);

			routes.MapRoute
			(
				"Directory",
				"Directory/{action}/{period}",
				new { controller = "Directory", action = "BestOverall", period = UrlParameter.Optional }
			);

			routes.MapRoute
			(
				"Default",
				"{controller}/{action}",
				new { controller = "Directory", action = "BestOverall" }
			);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RegisterRoutes(RouteTable.Routes);

			//XmlConfigurator.Configure();
			TraceAppender appender = new TraceAppender();
			appender.Layout = new log4net.Layout.PatternLayout("%date [%thread] %-5level %logger [%property{NDC}] - %message%newline");
			appender.ActivateOptions();
			BasicConfigurator.Configure(appender);
		}

		protected void Session_Start(object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{

		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}