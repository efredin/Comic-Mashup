using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Fredin.Util;
using System.Data;
using System.Data.Objects.DataClasses;

namespace Fredin.Comic.Data
{
	public partial class ComicModelContext
	{
		#region [User]

		public User TryGetUser(long uid)
		{
			return this.Users.FirstOrDefault(u => u.Uid == uid);
		}

		#endregion

		#region [Comic]

		public IQueryable<Comic> ListPublishedComics(User reader, List<long> friends, ComicStat.ComicStatPeriod period)
		{
			DateTime cutoff = Data.ComicStat.PeriodToCutoff(period);

			return this.Comics
				.Where(c => c.IsPublished && c.PublishTime.Value >= cutoff)
				.FilterComicVisibility(reader, friends);
				//.IncludeAuthor()
				//.IncludeStats();
		}

		public IQueryable<Comic> ListPublishedComics(User author, User reader, bool isFriend, ComicStat.ComicStatPeriod period)
		{
			DateTime cutoff = Data.ComicStat.PeriodToCutoff(period);

			return this.Comics
				.Where(c => c.IsPublished && c.PublishTime.Value >= cutoff)
				.FilterComicVisibility(reader, isFriend);
				//.IncludeAuthor()
				//.IncludeStats();
		}

		public IQueryable<Comic> SearchPublishedComics(string search, User reader, List<long> friends, ComicStat.ComicStatPeriod period)
		{
			DateTime cutoff = Data.ComicStat.PeriodToCutoff(period);

			return this.Comics
				.Where(c => c.IsPublished && c.PublishTime.Value >= cutoff && (c.Title.Contains(search) || c.Description.Contains(search)))
				.FilterComicVisibility(reader, friends);
		}

		public IQueryable<Comic> ListFeaturedComics(User reader)
		{
			return this.Comics
				.Where(c => c.FeatureTime.HasValue)
				.FilterComicVisibility(reader);
				//.IncludeAuthor()
				//.IncludeStats();
		}

		public Comic TryGetUnpublishedComic(long comicId, User author)
		{
			return this.Comics
				.FirstOrDefault(c => c.ComicId == comicId && c.Uid == author.Uid);
		}

		public Comic TryGetComic(long comicId, User reader)
		{
			return this.TryGetComic(comicId, reader, true);
		}

		public Comic TryGetComic(long comicId, User reader, bool isFriend)
		{
			return this.Comics
				.FilterComicVisibility(reader, isFriend)
				//.IncludeAuthor()
				//.IncludeStats()
				.FirstOrDefault(c => c.ComicId == comicId);
		}

		public Comic TryGetRandomComic(User reader, List<long> friends)
		{
			return this.Comics
				.FilterComicVisibility(reader, friends)
				.OrderBy(c => Guid.NewGuid())
				.FirstOrDefault();
		}

		public void PublishComic(Comic comic, User author)
		{
			if (comic.Author.Uid != author.Uid)
			{
				throw new UnauthorizedAccessException("Only the author of a comic may publish it.");
			}

			comic.IsPublished = true;
			comic.PublishTime = DateTime.Now;
		}

		public ComicRead TryGetComicRead(Comic comic, User reader)
		{
			ComicRead read = null;
			if (reader != null)
			{
				read = this.ComicRead.FirstOrDefault(r => r.ComicId == comic.ComicId && r.Uid == reader.Uid);
			}
			return read;
		}

		#endregion

		#region [Template]

		public IQueryable<Template> ListTemplates()
		{
			return this.Templates
				.Include(t => t.TemplateItems)
				.Where(t => !t.IsDeleted);
		}

		#endregion

		#region [Bubbles]

		public IQueryable<TextBubble> ListTextBubbles()
		{
			return this.TextBubbles
				.Include(b => b.TextBubbleDirections)
				.Where(b => !b.IsDeleted);
		}

		#endregion

		protected override void Dispose(bool disposing)
		{
			foreach (var entry in this.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Deleted | EntityState.Modified | EntityState.Unchanged))
			{
				if (entry.Entity != null)
				{
					// Set the change tracker to null
					IEntityWithChangeTracker entity = (IEntityWithChangeTracker)entry.Entity;
					entity.SetChangeTracker(null);
				}
			}
			base.Dispose(disposing);
		}
	}
}