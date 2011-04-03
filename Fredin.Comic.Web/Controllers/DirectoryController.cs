using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fredin.Comic.Web.Models;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Controllers
{
    public class DirectoryController : ComicControllerBase
    {
		public ActionResult BestOverall(ComicStat.ComicStatPeriod? period)
		{
			if (!period.HasValue)
			{
				period = ComicStat.ComicStatPeriod.AllTime;
			}

			var comics = this.EntityContext.ListPublishedComics(this.ActiveUser, this.Friends)
				.OrderByDescending(c => c.ComicStat.FirstOrDefault(s => s.Period == (int)period).Overall)
				.ToList();

			ViewDirectory view = new ViewDirectory(comics, ViewDirectory.DirectoryMode.BestOverall);
			return View("Directory", view);
		}

		public ActionResult Funniest(ComicStat.ComicStatPeriod? period)
		{
			if (!period.HasValue)
			{
				period = ComicStat.ComicStatPeriod.AllTime;
			}

			var comics = this.EntityContext.ListPublishedComics(this.ActiveUser, this.Friends)
				.OrderByDescending(c => c.ComicStat.FirstOrDefault(s => s.Period == (int)period).Funny)
				.ToList();

			ViewDirectory view = new ViewDirectory(comics, ViewDirectory.DirectoryMode.Funniest);
			return View("Directory", view);
		}

		public ActionResult Smartest(ComicStat.ComicStatPeriod? period)
		{
			if (!period.HasValue)
			{
				period = ComicStat.ComicStatPeriod.AllTime;
			}

			var comics = this.EntityContext.ListPublishedComics(this.ActiveUser, this.Friends)
				.OrderByDescending(c => c.ComicStat.FirstOrDefault(s => s.Period == (int)period).Smart)
				.ToList();

			ViewDirectory view = new ViewDirectory(comics, ViewDirectory.DirectoryMode.Smartest);
			return View("Directory", view);
		}

		public ActionResult MostRandom(ComicStat.ComicStatPeriod? period)
		{
			if (!period.HasValue)
			{
				period = ComicStat.ComicStatPeriod.AllTime;
			}

			var comics = this.EntityContext.ListPublishedComics(this.ActiveUser, this.Friends)
				.OrderByDescending(c => c.ComicStat.FirstOrDefault(s => s.Period == (int)period).Random)
				.ToList();

			ViewDirectory view = new ViewDirectory(comics, ViewDirectory.DirectoryMode.BestRandom);
			return View("Directory", view);
		}
    }
}
