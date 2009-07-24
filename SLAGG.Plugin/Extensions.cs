using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.Plugin
{
	public static class Extensions
	{
		public static int Trim (this int self, int max)
		{
			return (self > max) ? max : self;
		}

		public static string Explode<T> (this IEnumerable<T> self, string separator)
		{
			StringBuilder builder = new StringBuilder();

			foreach (T item in self)
			{
				if (builder.Length > 0)
					builder.Append (separator);

				builder.Append (item.ToString());
			}

			return builder.ToString();
		}
	}
}