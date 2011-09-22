using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fredin.Comic.Data
{
	public partial class UserEngageHistory
	{
		public static class EngagementType
		{
			public const string Comment = "comment";
			public const string ComicCreate = "create";
			public const string ComicRemix = "remix";
			public const string News = "news";
			public const string ComicUnpublished = "invite"; // Invite new users after a few days to create & publish comics
			public const string ProfileRender = "profile";
		}
	}
}
