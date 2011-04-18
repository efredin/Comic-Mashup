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
		public bool Vote { get; set; }
		public bool Tag { get; set; }

		public ClientUserEngage(UserEngage engage)
		{
			this.Uid = engage.Uid;
			this.Comment = engage.Comment;
			this.Vote = engage.Vote;
			this.Tag = engage.Tag;
		}
	}
}