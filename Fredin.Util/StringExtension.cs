using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Fredin.Util
{
	public static class StringExtension
	{
		public static string StripHtml(this string source)
		{
			return Regex.Replace(source, "<.*?>", string.Empty);
		}

		public static string FirstToLower(this string value)
		{
			string first = String.Empty;
			string last = String.Empty;

			if (value.Length >= 1)
			{
				first = value.Substring(0, 1).ToLower();
			}
			if (value.Length >= 2)
			{
				last = value.Substring(1);
			}

			return String.Format("{0}{1}", first, last);
		}

		public static string ComputeMd5(this string value)
		{
			System.Security.Cryptography.MD5CryptoServiceProvider x = new System.Security.Cryptography.MD5CryptoServiceProvider();
			byte[] buffer = System.Text.Encoding.UTF8.GetBytes(value);
			buffer = x.ComputeHash(buffer);
			System.Text.StringBuilder s = new System.Text.StringBuilder();
			foreach (byte b in buffer)
			{
				s.Append(b.ToString("x2").ToLower());
			}
			return s.ToString();
		}

		public static string ReplaceAt(this string source, int index, char replacement)
		{
			var temp = source.ToCharArray();
			temp[index] = replacement;
			return new String(temp);
		}
	}
}
