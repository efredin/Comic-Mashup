using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Data
{
	public partial class Comic
	{
		public ComicStat PeriodStats(ComicStat.ComicStatPeriod period)
		{
			ComicStat stat = this.ComicStat.FirstOrDefault(s => s.Period == (int)period);
			if (stat == null)
			{
				stat = new ComicStat()
				{
					ComicId = this.ComicId,
					Period = (int)period
				};
			}
			return stat;
		}
	}
}