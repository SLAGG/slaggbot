using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using SLAGG.Plugin.Configuration;

namespace SLAGG.RSS
{
	public class RssFeedElement
		: ConfigurationElement, IConfigElement<string>
	{
		[ConfigurationProperty ("name", IsRequired = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		[ConfigurationProperty ("feedurl", IsRequired = true)]
		public string FeedURL
		{
			get { return (string)this["feedurl"]; }
			set { this["feedurl"] = value; }
		}

		#region IConfigElement Members

		string IConfigElement<string>.Key
		{
			get { return this.Name; }
		}

		#endregion
	}
}