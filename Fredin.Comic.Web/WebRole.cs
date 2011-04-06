using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using log4net.Appender;
using log4net.Config;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.Diagnostics.Management;
using Microsoft.WindowsAzure.ServiceRuntime;
using log4net;

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

			string wadConnectionString = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(RoleEnvironment.GetConfigurationSettingValue(wadConnectionString));
			RoleInstanceDiagnosticManager roleInstanceDiagnosticManager = storageAccount.CreateRoleInstanceDiagnosticManager(RoleEnvironment.DeploymentId, RoleEnvironment.CurrentRoleInstance.Role.Name, RoleEnvironment.CurrentRoleInstance.Id);
			DiagnosticMonitorConfiguration config = roleInstanceDiagnosticManager.GetCurrentConfiguration();

			config.Logs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1D);
			config.Logs.ScheduledTransferLogLevelFilter = LogLevel.Undefined;
			config.DiagnosticInfrastructureLogs.ScheduledTransferLogLevelFilter = LogLevel.Warning;
			config.DiagnosticInfrastructureLogs.ScheduledTransferPeriod = TimeSpan.FromMinutes(1D);
			config.Directories.ScheduledTransferPeriod = TimeSpan.FromMinutes(1D);

			roleInstanceDiagnosticManager.SetCurrentConfiguration(config);
			
			return base.OnStart();
		}

		public override void Run()
		{
			base.Run();
		}
	}
}
