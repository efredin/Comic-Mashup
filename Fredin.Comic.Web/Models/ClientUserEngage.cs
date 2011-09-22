using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ClientUserEngage
	{
		public long Uid { get; set; }
		public bool Comment { get; set; }
		public bool ComicCreate { get; set; }
		public bool ComicRemix { get; set; }
		public bool Unsubscribe { get; set; }
		public bool Subscribe
		{
			get { return !this.Unsubscribe; }
			set { this.Unsubscribe = !value; }
		}

		public ClientUserEngage()
		{
		}

		public ClientUserEngage(UserEngage engage)
		{
			this.Uid = engage.Uid;
			this.Comment = engage.Comment;
			this.ComicCreate = engage.ComicCreate;
			this.ComicRemix = engage.ComicRemix;
			this.Unsubscribe = engage.Unsubscribe;
		}
	}
}