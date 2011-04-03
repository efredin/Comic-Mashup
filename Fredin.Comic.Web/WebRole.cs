using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

using log4net.Config;
using log4net.Appender;

namespace Fredin.Comic.Web
{
	public class WebRole : RoleEntryPoint
	{
		public override bool OnStart()
		{
			CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
			{
				configSetter(RoleEnvironment.GetConfigurationSettingValue(configName));
			});

			//// Role and IIS execute under a different domain on Azure so we cannot see the web.config file from here.
			//// Manually configure a trace appender for log4net to use
			//TraceAppender appender = new TraceAppender();
			//appender.ActivateOptions();
			//BasicConfigurator.Configure(appender);

			return base.OnStart();
		}

		public override void Run()
		{
			base.Run();
		}
	}
}
