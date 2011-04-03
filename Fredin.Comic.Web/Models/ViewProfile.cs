using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ViewProfile
	{
		public ClientUser User { get; set; }
		public List<ClientComic> Comics { get; set; }

		public ViewProfile(ClientUser user, List<ClientComic> comics)
		{
			this.User = user;
			this.Comics = comics;
		}

		public ViewProfile(User user, List<Data.Comic> comics)
		{
			this.User = new ClientUser(user);
			this.Comics = comics.Select(c => new ClientComic(c)).ToList();
		}
	}
}