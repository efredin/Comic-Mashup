using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Fredin.Comic.Data
{
	public partial class ComicStat
	{
		public enum ComicStatPeriod
		{
			[Description("All Time")]
			AllTime,
			[Description("This Year")]
			Year,
			[Description("This Month")]
			Month,
			[Description("This Week")]
			Week,
			[Description("Today")]
			Day
		}

		public static DateTime PeriodToCutoff(ComicStat.ComicStatPeriod period)
		{
			DateTime cutoff;
			switch (period)
			{
				case ComicStat.ComicStatPeriod.AllTime:
					cutoff = new DateTime(2011, 1, 1);
					break;

				case ComicStat.ComicStatPeriod.Year:
					cutoff = new DateTime(DateTime.Now.Year, 1, 1);
					break;

				case ComicStat.ComicStatPeriod.Month:
				default:
					cutoff = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
					break;

				case ComicStat.ComicStatPeriod.Week:
					cutoff = DateTime.Now.Date.AddDays((int)DateTime.Now.DayOfWeek * -1);
					break;

				case ComicStat.ComicStatPeriod.Day:
					cutoff = DateTime.Now.Date;
					break;
			}
			return cutoff;
		}
	}
}