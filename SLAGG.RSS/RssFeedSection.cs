using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SLAGG.RSS
{
	public class RssFeedSection
		: ConfigurationSection
	{
		[ConfigurationProperty ("feeds", IsDefaultCollection = true)]
		[ConfigurationCollection (typeof (RssFeedCollection), AddItemName = "addfeed", ClearItemsName="clearfeeds", RemoveItemName="removefeed")]
		public RssFeedCollection Feeds
		{
			get { return (RssFeedCollection)base["feeds"]; }
		}
	}
}