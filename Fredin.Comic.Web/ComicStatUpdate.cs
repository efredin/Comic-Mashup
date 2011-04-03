using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using Fredin.Comic.Web.Models;
using log4net;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web
{
	public class ComicStatUpdate
	{
		#region [Singelton]
		private static ComicStatUpdate _instance;
		public static ComicStatUpdate Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = new ComicStatUpdate();
				}
				return _instance;
			}
		}
		static ComicStatUpdate()
		{
			Log = LogManager.GetLogger(typeof(ComicStatUpdate));
		}
		#endregion

		private static ILog Log { get; set; }
		private System.Threading.Timer UpdateTimer { get; set; }

		private ComicStatUpdate()
		{
		}

		public void Start()
		{
			this.UpdateTimer = new System.Threading.Timer(delegate(object state)
			{
				// Attempt to find a connection string matching the current namespace
				ConnectionStringSettings connectionString = ConfigurationManager.ConnectionStrings[typeof(ComicStatUpdate).Namespace];
				if (connectionString != null)
				{
					Log.InfoFormat("Entity context using connection string '{0}'.", this.GetType().Namespace);
				}
				else
				{
					throw new Exception(String.Format("Unable to find connection string '{0}' for entity context.", typeof(ComicStatUpdate).Namespace));
				}

				using (ComicModelContext context = new ComicModelContext(connectionString.ConnectionString))
				{
					context.UpdateComicStat((int)ComicStat.ComicStatPeriod.AllTime, ComicStat.PeriodToCutoff(ComicStat.ComicStatPeriod.AllTime));
				}

			}, null, new TimeSpan(0, 0, 0), new TimeSpan(0, 15, 0));

		}

		public void Stop()
		{
			this.UpdateTimer.Dispose();
			this.UpdateTimer = null;
		}
	}
}