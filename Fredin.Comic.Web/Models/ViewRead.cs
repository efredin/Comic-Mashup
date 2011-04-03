using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ViewRead
	{
		public ClientComic Comic { get; set; }
		public ClientComicRead Reader { get; set; }

		public ViewRead(Data.Comic comic, ComicRead reader)
		{
			this.Comic = new ClientComic(comic);
			this.Reader = new ClientComicRead(reader);
		}
	}
}