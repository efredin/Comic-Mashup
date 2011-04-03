using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ClientComicRead
	{
		public long Uid { get; set; }
		public long ComicId { get; set; }
		public DateTime ReadTime { get; set; }
		public bool IsFunny { get; set; }
		public bool IsSmart { get; set; }
		public bool IsRandom { get; set; }

		public ClientComicRead(ComicRead source)
		{
			this.Uid = source.Uid;
			this.ComicId = source.ComicId;
			this.ReadTime = source.ReadTime;
			this.IsFunny = source.IsFunny;
			this.IsSmart = source.IsSmart;
			this.IsRandom = source.IsRandom;
		}
	}
}