using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Fredin.Util
{
	public static class CultureInfoExtension
	{
		public static string ShortDisplayName(this CultureInfo culture)
		{
			return Regex.Replace(culture.DisplayName, @"\w?\([^)]*\)", String.Empty, RegexOptions.IgnoreCase);
		}
	}
}
