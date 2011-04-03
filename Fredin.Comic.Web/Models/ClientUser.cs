using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	[Serializable]
	public class ClientUser
	{
		public long Uid { get; set; }
		public string Name { get; set; }
		public string Nickname { get; set; }
		public string FbLink { get; set; }

		public string ProfileUrl { get; set; }
		public string ThumbUrl { get; set; }

		public ClientUser(User source)
		{
			this.Uid = source.Uid;
			this.Name = source.Name;
			this.Nickname = source.Nickname;
			this.FbLink = source.FbLink;

			this.ProfileUrl = ComicUrlHelper.GetProfileUrl(source);
			this.ThumbUrl = String.Format("http://graph.facebook.com/{0}/picture", source.Uid);
		}
	}
}