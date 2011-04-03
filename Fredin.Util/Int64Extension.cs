using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fredin.Util
{
	public static class Int64Extension
	{
		public static DateTime UnixTimeAsDateTime(this long timestamp)
		{
			DateTime posix = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return posix.AddSeconds(timestamp);
		}
	}
}
