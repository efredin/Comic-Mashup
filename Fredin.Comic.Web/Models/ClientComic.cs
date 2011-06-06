using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Comic.Data;
using Fredin.Comic.Render;

namespace Fredin.Comic.Web.Models
{
	public class ClientComic
	{
		public long ComicId { get; set; }
		public long Uid { get; set; }
		public long TemplateId { get; set; }
		public DateTime CreateTime { get; set; }
		public DateTime UpdateTime { get; set; }
		public DateTime? PublishTime { get; set; }
		public DateTime? FeatureTime { get; set; }
		public bool IsPublished { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string ShareText { get; set; }
		public bool IsPrivate { get; set; }

		public string ReadUrl { get; set; }
		public string ComicUrl { get; set; }
		public string ThumbUrl { get; set; }
		public string FrameUrl { get; set; }
		public string FrameThumbUrl { get; set; }
		public string RemixUrl { get; set; }

		public ClientUser Author { get; set; }
		public List<ClientComicTextBubble> Bubbles { get; set; }
		public List<ClientPhoto> Photos { get; set; }
		public ClientTemplate Template { get; set; }
		public ClientComicStat Stats { get; set; }

		public ClientComic(Data.Comic source)
		{
			this.Init(source, ComicStat.ComicStatPeriod.AllTime);
		}

		public ClientComic(Data.Comic source, ComicStat.ComicStatPeriod statsPeriod)
		{
			this.Init(source, statsPeriod);
		}

		protected void Init(Data.Comic source, ComicStat.ComicStatPeriod statsPeriod)
		{
			this.ComicId = source.ComicId;
			this.Uid = source.Uid;
			this.TemplateId = source.TemplateId;
			this.CreateTime = source.CreateTime;
			this.UpdateTime = source.UpdateTime;
			this.PublishTime = source.PublishTime;
			this.FeatureTime = source.FeatureTime;
			this.IsPublished = source.IsPublished;
			this.Title = source.Title;
			this.Description = source.Description;
			this.ShareText = source.ShareText;
			this.IsPrivate = source.IsPrivate;

			this.ReadUrl = ComicUrlHelper.GetReadUrl(source);
			this.ComicUrl = ComicUrlHelper.GetRenderUrl(source, RenderMode.Comic);
			this.ThumbUrl = ComicUrlHelper.GetRenderUrl(source, RenderMode.Thumb);
			this.FrameUrl = ComicUrlHelper.GetRenderUrl(source, RenderMode.Frame);
			this.FrameThumbUrl = ComicUrlHelper.GetRenderUrl(source, RenderMode.FrameThumb);
			this.RemixUrl = ComicUrlHelper.GetRemixUrl(source);

			this.Author = new ClientUser(source.Author);
			this.Stats = new ClientComicStat(source.PeriodStats(statsPeriod));
			this.Template = new ClientTemplate(source.Template);

			if(source.ComicTextBubbles.IsLoaded)
			{
				this.Bubbles = source.ComicTextBubbles.ToList().Select(b => new ClientComicTextBubble(b)).ToList();
			}
			if (source.ComicPhotos.IsLoaded)
			{
				this.Photos = source.ComicPhotos
					.OrderBy(p => p.TemplateItem.Ordinal)
					.ToList()
					.Select(p => new ClientPhoto(p.Photo))
					.ToList();
			}
		}
	}
}