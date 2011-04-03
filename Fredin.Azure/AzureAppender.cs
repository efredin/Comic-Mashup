using System;
using System.Diagnostics;
using System.Linq;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace Fredin.Azure
{
	public sealed class AzureAppender : AppenderSkeleton
	{
		private const string KeyConnectionString = "Diagnostics.ConnectionString";
		private const string KeyScheduledTransferPeriod = "Diagnostics.ScheduledTransferPeriod";
		private const string KeyEventLogs = "Diagnostics.EventLogs";
		private const string KeyLevel = "Diagnostics.Level";

		public string Level
		{
			get
			{
				try
				{
					return RoleEnvironment.GetConfigurationSettingValue(KeyLevel);
				}
				catch
				{
					return "ALL";
				}
			}
		}

		public string EventLogs
		{
			get
			{
				try
				{
					return RoleEnvironment.GetConfigurationSettingValue(KeyEventLogs);
				}
				catch
				{
					return "Application!*;System!*";
				}
			}
		}

		public int ScheduledTransferPeriod
		{
			get
			{
				try
				{
					return int.Parse(RoleEnvironment.GetConfigurationSettingValue(KeyScheduledTransferPeriod));
				}
				catch
				{
					return 5;
				}
			}
		}

		protected override void Append(LoggingEvent loggingEvent)
		{
			System.Diagnostics.Trace.WriteLine(this.RenderLoggingEvent(loggingEvent));
		}

		public override void ActivateOptions()
		{
			Hierarchy rootRepository = (Hierarchy)log4net.LogManager.GetRepository();
			this.Threshold = rootRepository.LevelMap[this.Level];

			base.ActivateOptions();
			this.ConfigureAzureDiagnostics();
		}

		private void ConfigureAzureDiagnostics()
		{
			var traceListener = new DiagnosticMonitorTraceListener();
			Trace.Listeners.Add(traceListener);

			var dmc = DiagnosticMonitor.GetDefaultInitialConfiguration();

			//set threshold to verbose, what gets logged is controled by the log4net level
			dmc.Logs.ScheduledTransferLogLevelFilter = LogLevel.Verbose;

			TimeSpan transferPeriod = TimeSpan.FromMinutes(this.ScheduledTransferPeriod);
			dmc.Logs.ScheduledTransferPeriod = transferPeriod;
			dmc.WindowsEventLog.ScheduledTransferPeriod = transferPeriod;

			// (;) delimited list of event logs
			foreach (string log in this.EventLogs.Split(';'))
			{
				dmc.WindowsEventLog.DataSources.Add(log);
			}

			DiagnosticMonitor.Start(KeyConnectionString, dmc);
		}
	}
}