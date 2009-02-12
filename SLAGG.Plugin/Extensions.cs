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
	}
}