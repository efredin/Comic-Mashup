using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;

namespace Fredin.Comic.Web.Models
{
	public class ClientTemplate
	{
		public long TemplateId { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Ordinal { get; set; }
		public string Size { get; set; }
		public bool IsDeleted { get; set; }

		public string ThumbUrl { get; set; }
		public int FrameCount { get; set; }

		public List<ClientTemplateItem> TemplateItems { get; set;}

		public ClientTemplate(Template source)
		{
			this.TemplateId = source.TemplateId;
			this.Width = source.Width;
			this.Height = source.Height;
			this.Ordinal = source.Ordinal;
			this.Size = source.Size;
			this.IsDeleted = source.IsDeleted;

			this.ThumbUrl = ComicUrlHelper.GetImageUrl(source.Thumb);
			this.FrameCount = source.TemplateItems.Count;

			this.TemplateItems = source.TemplateItems
				.ToList()
				.Select(i => new ClientTemplateItem(i))
				.OrderBy(i => i.Ordinal)
				.ToList();
		}
	}
}