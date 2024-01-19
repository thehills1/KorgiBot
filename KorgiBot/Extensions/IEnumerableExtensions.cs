using System;
using System.Collections.Generic;
using System.Linq;

namespace KorgiBot.Extensions
{
	public static class IEnumerableExtensions
	{
		public static bool ContainsAny<T>(this IEnumerable<T> source, IEnumerable<T> values)
		{
			if (!values?.Any() ?? true) return false;

			var contains = false;
			foreach (var value in values)
			{
				if (source.Contains(value))
				{
					contains = true; 
					break;
				}
			}

			return contains;
		}
	}
}
