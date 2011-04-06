using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ViewDirectory
	{
		public List<ClientComic> Comics { get; set; }
		public DirectoryMode Mode { get; set; }
		public ComicStat.ComicStatPeriod Period { get; set; }
		
		/// <summary>
		/// 1-based page index
		/// </summary>
		public int Page { get; set; }

		/// <summary>
		/// 1-based max page index
		/// </summary>
		public int MaxPage { get; set; }

		public ViewDirectory(List<Data.Comic> comics, DirectoryMode mode, ComicStat.ComicStatPeriod period, int page, int maxPage)
		{
			this.Comics = comics.Select(c => new ClientComic(c)).ToList();
			this.Mode = mode;
			this.Period = period;
			this.Page = page;
			this.MaxPage = maxPage;
		}

		public enum DirectoryMode
		{
			[Description("Best Overall")]
			BestOverall,

			[Description("Funniest")]
			Funniest,

			[Description("Smartest")]
			Smartest,

			[Description("Most Random")]
			MostRandom,

			[Description("Newest")]
			Newest,

			[Description("Search")]
			Search
		}
	}
}