using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace Fredin.Util
{
	public static class BinaryExtension
	{
		public static ulong ToUlong(this Binary binary)
		{
			return BitConverter.ToUInt64(binary.ToArray(), 0);
		}

		public static Binary ToBinary(this ulong value)
		{
			return new Binary(BitConverter.GetBytes(value));
		}
	}
}
