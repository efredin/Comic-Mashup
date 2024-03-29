﻿using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web;

namespace Fredin.Comic.Data
{
	public static class ComicModelExtensions
	{
		#region [Comic]
		//public static ObjectSet<Comic> IncludeComicStuff()
		//{
		//    return null;
		//}

		//public static Comic EnsureComicVisiblity(this Comic comic, User user, bool isFriend)
		//{
		//    bool visible = false;

		//    // Deleted or non-existent
		//    if (!comic.IsDeleted)
		//    {
		//        // Owner
		//        if (user != null && comic.UserId == user.UserId)
		//        {
		//            visible = true;
		//        }

		//        // Published
		//        if (comic.IsPublished)
		//        {
		//            visible = true;
		//        }
		//    }

		//    if (!visible)
		//    {
		//        throw new ComicVisibilityException("The current user is not authorized to view the requested comic.");
		//    }
		//    return comic;
		//}

		public static IQueryable<Comic> FilterComicVisibility(this IQueryable<Comic> comics, User user)
		{
			return comics.FilterComicVisibility(user, false);
		}

		public static IQueryable<Comic> FilterComicVisibility(this IQueryable<Comic> comics, User user, bool isFriend)
		{
			IQueryable<Comic> filtered;

			if (user != null)
			{
				filtered = comics.Where(c => c.Uid == user.Uid || !c.IsPrivate || isFriend);
			}
			else
			{
				filtered = comics.Where(c => !c.IsPrivate || isFriend);
			}

			return filtered.Where(c => c.IsPublished && !c.IsDeleted && !c.Author.IsDeleted);
		}

		public static IQueryable<Comic> FilterComicVisibility(this IQueryable<Comic> comics, User user, List<long> friends)
		{
			IQueryable<Comic> filtered;

			if (user != null)
			{
				filtered = comics.Where(c => c.Uid == user.Uid || !c.IsPrivate || friends.Contains(c.Uid));
			}
			else
			{
				filtered = comics.Where(c => !c.IsPrivate);
			}

			return filtered.Where(c => c.IsPublished && !c.IsDeleted && !c.Author.IsDeleted);
		}

		public static IQueryable<Comic> FilterComicLanguage(this IQueryable<Comic> comics, string languageCode)
		{
			IQueryable<Comic> filtered = comics;
			if (languageCode != null)
			{
				filtered = filtered.Where(c => c.Locale.Substring(0, 2).ToLower() == languageCode.ToLower());
			}
			return filtered;
		}

		//public static IQueryable<Comic> IncludeStats(this IQueryable<Comic> comics)
		//{
		//    foreach (Comic c in comics)
		//    {
		//        c.ComicStat.Load();
		//    }
		//    return comics;
		//}

		//public static IQueryable<Comic> IncludeAuthor(this IQueryable<Comic> comics)
		//{
		//    foreach (Comic c in comics)
		//    {
		//        c.AuthorReference.Load();
		//    }
		//    return comics;
		//}

		#endregion
	}
}