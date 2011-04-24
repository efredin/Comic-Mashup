using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.StorageClient;
using log4net;
using log4net.Config;
using Microsoft.WindowsAzure.Diagnostics.Management;

namespace Fredin.Comic.Worker
{
	public class WorkerRole : RoleEntryPoint
	{
		public override void Run()
		{
			// This is a sample worker implementation. Replace with your logic.
			Trace.WriteLine("Fredin.Comic.Worker entry point called", "Information");

			while (true)
			{
				Thread.Sleep(10000);
			}
		}

		public override bool OnStart()
		{
			// Set the maximum number of concurrent connections 
			ServicePointManager.DefaultConnectionLimit = 12;

			XmlConfigurator.Configure();
			ILog log = LogManager.GetLogger(typeof(WorkerRole));

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


			PhotoTaskManager.Instance.Start();
			RenderTaskManager.Instance.Start();
			ProfileTaskManager.Instance.Start();
			StatUpdate.Instance.Start();

			return base.OnStart();
		}

		public override void OnStop()
		{
			StatUpdate.Instance.Stop();
			ProfileTaskManager.Instance.Stop();
			RenderTaskManager.Instance.Stop();
			PhotoTaskManager.Instance.Stop();
			base.OnStop();
		}
	}
}
