using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Fredin.Comic.Web.Models;
using Fredin.Comic.Data;
using System.Linq.Expressions;
using System.Globalization;
using Fredin.Util;

namespace Fredin.Comic.Web.Controllers
{
    public class DirectoryController : ComicControllerBase
    {
		protected const int PageSize = 12;

		public ActionResult Index()
		{
			var comics = this.EntityContext.ListPublishedComics(this.ActiveUser, this.Friends, ComicStat.ComicStatPeriod.AllTime, CultureInfo.CurrentCulture.TwoLetterISOLanguageName)
				.OrderByDescending(c => c.PublishTime.Value)
				.Take(10)
				.ToList();

			ViewDirectory view = new ViewDirectory(comics, ViewDirectory.DirectoryMode.Newest, ComicStat.ComicStatPeriod.AllTime, 1, 1);
			return View("Home", view);
		}

		public ActionResult Home()
		{
			return this.RedirectToAction("Index");
		}

		protected ViewDirectory LoadDirectory(ComicStat.ComicStatPeriod? period, string language, int? page, Expression<Func<Data.Comic, long>> sortExpression, ViewDirectory.DirectoryMode mode)
		{
			if (!period.HasValue) period = ComicStat.ComicStatPeriod.AllTime;
			page = Math.Max(page.HasValue ? page.Value : 1, 1);
			int skip = Math.Max(page.Value - 1, 0) * PageSize;
			if (String.IsNullOrWhiteSpace(language)) language = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

			IQueryable<Data.Comic> query = this.EntityContext.ListPublishedComics(this.ActiveUser, this.Friends, period.Value, language);
			
			List<Data.Comic> comics = query
				.OrderByDescending(sortExpression)
				.Skip(skip)
				.Take(PageSize)
				.ToList();

			int items = query.Count();
			int max = items / PageSize;
			if (items % PageSize != 0) max += 1;

			return new ViewDirectory(comics, mode, period.Value, page.Value, max);
		}

		public ActionResult BestOverall(ComicStat.ComicStatPeriod? period, string language, int? page)
		{
			if (!period.HasValue) period = ComicStat.ComicStatPeriod.AllTime;
			ViewDirectory view = this.LoadDirectory(period, language, page, c => c.ComicStat.FirstOrDefault(s => s.Period == (int)period).Overall, ViewDirectory.DirectoryMode.BestOverall);
			return View("Directory", view);
		}

		public ActionResult Funniest(ComicStat.ComicStatPeriod? period, string language, int? page)
		{
			if (!period.HasValue) period = ComicStat.ComicStatPeriod.AllTime;
			ViewDirectory view = this.LoadDirectory(period, language, page, c => c.ComicStat.FirstOrDefault(s => s.Period == (int)period).Funny, ViewDirectory.DirectoryMode.Funniest);
			return View("Directory", view);
		}

		public ActionResult Smartest(ComicStat.ComicStatPeriod? period, string language, int? page)
		{
			if (!period.HasValue) period = ComicStat.ComicStatPeriod.AllTime;
			ViewDirectory view = this.LoadDirectory(period, language, page, c => c.ComicStat.FirstOrDefault(s => s.Period == (int)period).Smart, ViewDirectory.DirectoryMode.Smartest);
			return View("Directory", view);
		}

		public ActionResult MostRandom(ComicStat.ComicStatPeriod? period, string language, int? page)
		{
			if (!period.HasValue) period = ComicStat.ComicStatPeriod.AllTime;
			ViewDirectory view = this.LoadDirectory(period, language, page, c => c.ComicStat.FirstOrDefault(s => s.Period == (int)period).Random, ViewDirectory.DirectoryMode.MostRandom);
			return View("Directory", view);
		}

		public ActionResult Newest(ComicStat.ComicStatPeriod? period, string language, int? page)
		{
			if (!period.HasValue) period = ComicStat.ComicStatPeriod.AllTime;
			ViewDirectory view = this.LoadDirectory(period, language, page, c => c.ComicId, ViewDirectory.DirectoryMode.Newest);
			return View("Directory", view);
		}

		public ActionResult Search(string search, ComicStat.ComicStatPeriod? period, string language, int? page)
		{
			if (!period.HasValue) period = ComicStat.ComicStatPeriod.AllTime;
			page = Math.Max(page.HasValue ? page.Value : 1, 1);
			int skip = Math.Max(page.Value - 1, 0) * PageSize;

			IQueryable<Data.Comic> query = this.EntityContext.SearchPublishedComics(search, this.ActiveUser, this.Friends, period.Value, language);

			List<Data.Comic> comics = query
				.OrderByDescending(c => c.ComicStat.FirstOrDefault(s => s.Period == (int)period).Overall)
				.Skip(skip)
				.Take(PageSize)
				.ToList();

			int items = query.Count();
			int max = items / PageSize;
			if (items % PageSize != 0) max += 1;

			ViewDirectory view = new ViewDirectory(comics, ViewDirectory.DirectoryMode.Search, period.Value, page.Value, max);
			return View("Directory", view);
		}

		public ActionResult Author(long? id, string title)
		{
			ActionResult result;

			User user = null;

			try
			{
				this.EntityContext.TryAttach(this.ActiveUser);

				if (!id.HasValue && this.ActiveUser != null)
				{
					user = this.ActiveUser;
					this.EntityContext.TryAttach(user);
				}
				else if (id.HasValue)
				{
					user = this.EntityContext.TryGetUser(id.Value);
				}

				if (user != null)
				{
					// Load published comics
					List<Data.Comic> comics = this.EntityContext.ListPublishedComics(user, this.ActiveUser, this.IsFriendOrSelf(user), ComicStat.ComicStatPeriod.AllTime, null)
							.OrderByDescending(c => c.PublishTime)
							.ToList();

					// Get stats for each comic
					if (this.Request[KEY_FORMAT] == VAL_JSON)
					{
						result = this.Json(new ViewAuthor(user, comics), JsonRequestBehavior.AllowGet);
					}
					else
					{
						result = this.View(new ViewAuthor(user, comics));
					}
				}
				else
				{
					if (id.HasValue)
					{
						throw new Exception("Unknown user.");
					}
					else
					{
						result = this.RedirectToAction("Login", "User");
					}
				}
			}
			finally
			{
				this.EntityContext.TryDetach(this.ActiveUser);
				this.EntityContext.TryDetach(user);
			}

			return result;
		}
	}
}
