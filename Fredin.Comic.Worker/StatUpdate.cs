using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using log4net;
using Fredin.Comic.Data;

namespace Fredin.Comic.Worker
{
	public class StatUpdate
	{
		#region [Singelton]
		private static StatUpdate _instance;
		public static StatUpdate Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new StatUpdate();
				}
				return _instance;
			}
		}
		static StatUpdate()
		{
			Log = LogManager.GetLogger(typeof(StatUpdate));
		}
		#endregion

		private static ILog Log { get; set; }
		private System.Threading.Timer UpdateTimer { get; set; }

		private StatUpdate()
		{
		}

		public void Start()
		{
			Log.Info("Starting");
			this.UpdateTimer = new System.Threading.Timer(delegate(object state)
			{
				// Attempt to find a connection string matching the current namespace
				ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings["ComicModelContext"];
				using (ComicModelContext context = new ComicModelContext(connectionString.ConnectionString))
				{
					context.UpdateComicStat((int)ComicStat.ComicStatPeriod.AllTime, ComicStat.PeriodToCutoff(ComicStat.ComicStatPeriod.AllTime));
					context.UpdateComicStat((int)ComicStat.ComicStatPeriod.Year, ComicStat.PeriodToCutoff(ComicStat.ComicStatPeriod.Year));
					context.UpdateComicStat((int)ComicStat.ComicStatPeriod.Month, ComicStat.PeriodToCutoff(ComicStat.ComicStatPeriod.Month));
					context.UpdateComicStat((int)ComicStat.ComicStatPeriod.Week, ComicStat.PeriodToCutoff(ComicStat.ComicStatPeriod.Week));
					context.UpdateComicStat((int)ComicStat.ComicStatPeriod.Day, ComicStat.PeriodToCutoff(ComicStat.ComicStatPeriod.Day));
				}

			}, null, new TimeSpan(0, 0, 0), new TimeSpan(0, 15, 0));
		}

		public void Stop()
		{
			Log.Info("Stopping");

			if (this.UpdateTimer != null)
			{
				this.UpdateTimer.Dispose();
				this.UpdateTimer = null;
			}
		}
	}
}