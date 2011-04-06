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

			RenderTaskManager.Instance.Start();
			StatUpdate.Instance.Start();

			return base.OnStart();
		}

		public override void OnStop()
		{
			StatUpdate.Instance.Stop();
			RenderTaskManager.Instance.Stop();
			base.OnStop();
		}
	}
}
