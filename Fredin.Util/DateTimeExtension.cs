using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fredin.Util
{
	public static class DateTimeExtension
	{
		private static TimeZoneInfo Edmonton { get; set; }

		private static readonly DateTime EPOC = new DateTime(1970, 1, 1);

		static DateTimeExtension()
		{
			// Define transition times to/from DST
			TimeZoneInfo.TransitionTime startTransition, endTransition;
			startTransition = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 4, 0, 0), 10, 2, DayOfWeek.Sunday);
			endTransition = TimeZoneInfo.TransitionTime.CreateFloatingDateRule(new DateTime(1, 1, 1, 3, 0, 0), 3, 2, DayOfWeek.Sunday);

			// Define adjustment rule
			TimeSpan delta = new TimeSpan(1, 0, 0);
			TimeZoneInfo.AdjustmentRule adjustment;
			adjustment = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(new DateTime(1999, 10, 1), DateTime.MaxValue.Date, delta, startTransition, endTransition);

			// Create array for adjustment rules
			TimeZoneInfo.AdjustmentRule[] adjustments = { adjustment };

			// Define other custom time zone arguments
			string displayName = "(GMT-07:00) Mountain/Edmonton Time";
			string standardName = "MST";
			string daylightName = "MDT";
			TimeSpan offset = new TimeSpan(-7, 0, 0);

			Edmonton = TimeZoneInfo.CreateCustomTimeZone(standardName, offset, displayName, standardName, daylightName, adjustments);
		}

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

		public static DateTime UtcToEdmonton(this DateTime targetTime)
		{
			return TimeZoneInfo.ConvertTimeFromUtc(targetTime, Edmonton);
		}
	}
}
