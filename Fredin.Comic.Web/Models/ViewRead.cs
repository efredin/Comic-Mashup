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
		public List<ClientComicTag> Tags { get; set; }

		public ViewRead(ClientComic comic, ClientComicRead reader, List<ClientComicTag> tags)
		{
			this.Comic = comic;
			this.Reader = reader;
			this.Tags = tags;
		}
	}
}