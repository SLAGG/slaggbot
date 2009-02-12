using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.Plugin.Configuration
{
	public interface IConfigElement<TKey>
	{
		TKey Key { get; }
	}
}