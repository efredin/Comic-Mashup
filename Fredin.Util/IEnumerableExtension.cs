using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Fredin.Util
{
	public static class IEnumerableExtension
	{
		public delegate void TraverseCallback(object current);

		public static void Traverse(this IEnumerable target, TraverseCallback callback)
		{
			IEnumerator enumerator = target.GetEnumerator();
			enumerator.Reset();
			while (enumerator.MoveNext())
			{
				callback.DynamicInvoke(enumerator.Current);
			}
		}

		public static double StdDev(this IEnumerable<double> values)
		{
			double stdDev = 0;
			if (values.Count() > 0)
			{
				double avg = values.Average();
				double sum = values.Sum(v => Math.Pow(v - avg, 2));
				stdDev = Math.Sqrt(sum / values.Count() - 1);
			}
			return stdDev;
		}
	}
}
