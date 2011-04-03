using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fredin.Util
{
	public static class DateTimeExtension
	{
		private static readonly DateTime EPOC = new DateTime(1970, 1, 1);

		public static string ToContextualTimeSpanString(this DateTime targetTime)
		{
			TimeSpan duration = DateTime.Now - targetTime;
			string output = String.Empty;

			// Year
			if (duration.Days > 365 * 2)
			{
				output = String.Format("{0:N0} years ago", duration.Days / 365);
			}
			if (duration.Days > 365)
			{
				output = String.Format("{0:N0} year ago", duration.Days / 365);
			}

			// Month
			else if (duration.Days > 30 * 2)
			{
				output = String.Format("{0:N0} months ago", duration.Days / 30);
			}
			else if (duration.Days > 30)
			{
				output = String.Format("{0:N0} month ago", duration.Days / 30);
			}

			// Day
			else if (duration.Days > 2)
			{
				output = String.Format("{0:N0} days ago", duration.Days);
			}
			else if (duration.Days > 1)
			{
				output = String.Format("{0:N0} day ago", duration.Days);
			}

			// Hour
			else if (duration.Hours > 2)
			{
				output = String.Format("{0:N0} hours ago", duration.Hours);
			}
			else if (duration.Hours > 1)
			{
				output = String.Format("{0:N0} hour ago", duration.Hours);
			}

			// Minute
			else if (duration.Minutes > 2)
			{
				output = String.Format("{0:N0} minutes ago", duration.Minutes);
			}
			else if (duration.Minutes > 1)
			{
				output = String.Format("{0:N0} minute ago", duration.Minutes);
			}

			// Less
			else
			{
				output = "moments ago";
			}

			return output;
		}

		public static long ToUnixTimestamp(this DateTime targetTime)
		{
			return (long)((TimeSpan)(targetTime - EPOC)).TotalSeconds;
		}
	}
}
