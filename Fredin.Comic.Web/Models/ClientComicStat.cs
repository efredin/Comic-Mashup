using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ClientComicStat
	{
		public long ComicId { get; set; }
		public int Overall { get; set; }
		public int Funny { get; set; }
		public int Smart { get; set; }
		public int Random { get; set; }
		public int Readers { get; set; }
		public ComicStat.ComicStatPeriod Period { get; set; }

		public string TopRating
		{
			get
			{
				string top = String.Empty;
				if (this.Funny >= this.Smart && this.Funny >= this.Random && this.Funny > 0)
				{
					top = "Funny";
				}
				else if (this.Smart >= this.Funny && this.Smart >= this.Random && this.Smart > 0)
				{
					top = "Smart";
				}
				else if (this.Random >= this.Funny && this.Random >= this.Smart && this.Random > 0)
				{
					top = "Random";
				}
				return top;
			}
		}

		public ClientComicStat(ComicStat source)
		{
			this.ComicId = source.ComicId;
			this.Funny = source.Funny;
			this.Smart = source.Smart;
			this.Random = source.Random;
			this.Overall = source.Overall;
			this.Period = (ComicStat.ComicStatPeriod)source.Period;
			this.Readers = source.Readers;
		}
	}
}