using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ClientComicTag
	{
		public long Uid { get; set; }
		public long ComicId { get; set; }
		public int? X { get; set; }
		public int? Y { get; set; }

		public string Nickname { get; set; }

		public ClientComicTag(ComicTag tag)
		{
			this.Uid = tag.Uid;
			this.ComicId = tag.ComicId;
			this.X = tag.X;
			this.Y = tag.Y;
			this.Nickname = tag.User.Nickname;
		}
	}
}