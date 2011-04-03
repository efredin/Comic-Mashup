using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace Fredin.Comic.Web.Models
{
	public class ViewDirectory
	{
		public List<ClientComic> Comics { get; set; }
		public DirectoryMode Mode { get; set; }

		public ViewDirectory(List<Data.Comic> comics, DirectoryMode mode)
		{
			this.Comics = comics.Select(c => new ClientComic(c)).ToList();
			this.Mode = mode;
		}

		public enum DirectoryMode
		{
			[Description("Best Overall")]
			BestOverall,

			[Description("Funniest")]
			Funniest,

			[Description("Smartest")]
			Smartest,

			[Description("Best Random")]
			BestRandom,

			[Description("Most Recent")]
			MostRecent
		}
	}
}