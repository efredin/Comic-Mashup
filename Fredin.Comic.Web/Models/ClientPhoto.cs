using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ClientPhoto
	{
		public long PhotoId { get; set; }
		public string ImageUrl { get; set; }
		public int Height { get; set; }
		public int Width { get; set; }

		public ClientPhoto(Data.Photo source)
		{
			this.ImageUrl = ComicUrlHelper.GetPhotoUrl(source);
			this.PhotoId = source.PhotoId;
			this.Height = source.Height;
			this.Width = source.Width;
		}
	}
}