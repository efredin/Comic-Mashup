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
using Microsoft.WindowsAzure;
using System.Configuration;
using Fredin.Comic.Config;
using log4net;

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
				"Directory/{action}/{period}/{page}",
				new { controller = "Directory", action = "BestOverall", period = UrlParameter.Optional, page = UrlParameter.Optional }
			);

			routes.MapRoute
			(
				"Index",
				"{controller}/{action}",
				new { controller = "Directory", action = "Index" }
			);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RegisterRoutes(RouteTable.Routes);

			XmlConfigurator.Configure();
			ILog log = LogManager.GetLogger(typeof(WebRole));

			//XmlConfigurator.Configure();
			//TraceAppender appender = new TraceAppender();
			//appender.Layout = new log4net.Layout.PatternLayout("%date [%thread] %-5level %logger [%property{NDC}] - %message%newline");
			//appender.ActivateOptions();
			//BasicConfigurator.Configure(appender);

			//ILog log = LogManager.GetLogger(typeof(Global));

			//// Ensure storage is initialized
			//log.Info("Initializing azure storage containers");
			//CloudStorageAccount account = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["ComicStorage"].ConnectionString);
			//CloudBlobClient blobClient = account.CreateCloudBlobClient();

			//// Render Container
			//CloudBlobContainer renderContainer = blobClient.GetContainerReference(ComicConfigSectionGroup.Blob.RenderContainer);
			//renderContainer.CreateIfNotExist();
			//BlobContainerPermissions renderPermission = new BlobContainerPermissions();
			//renderPermission.PublicAccess = BlobContainerPublicAccessType.Container;
			//renderContainer.SetPermissions(renderPermission);

			//// Task Container
			//CloudBlobContainer taskContainer = blobClient.GetContainerReference(ComicConfigSectionGroup.Blob.TaskContainer);
			//renderContainer.CreateIfNotExist();
		}

		protected void Session_Start(object sender, EventArgs e)
		{
		}

		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			HttpContext.Current.Response.AddHeader("p3p", "CP=\"CAO PSA OUR\"");
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