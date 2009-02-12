using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SLAGG.GameServers
{
	public class GameServersSection
		: ConfigurationSection
	{
		public GameServersSection ()
		{
			
		}

		[ConfigurationProperty ("servers", IsDefaultCollection = true)]
		[ConfigurationCollection (typeof (GameServersCollection), AddItemName="addserver", ClearItemsName="clearservers", RemoveItemName="removeserver")]
		public GameServersCollection Servers
		{
			get { return (GameServersCollection)base["servers"]; }
		}
	}

	public class GameServersCollection
		: ConfigurationElementCollection, IEnumerable<GameServerConfigElement>
	{
		protected override ConfigurationElement CreateNewElement ()
		{
			return new GameServerConfigElement ();
		}

		protected override object GetElementKey (ConfigurationElement element)
		{
			return ((GameServerConfigElement)element).Name;
		}

		public new GameServerConfigElement this[string name]
		{
			get { return (GameServerConfigElement)base.BaseGet (name); }
		}

		#region IEnumerable<GameServerConfigElement> Members

		public new IEnumerator<GameServerConfigElement> GetEnumerator ()
		{
			var enumerator = base.GetEnumerator ();
			enumerator.Reset ();

			while (enumerator.MoveNext())
				yield return (GameServerConfigElement)enumerator.Current;
		}

		#endregion
	}

	public class GameServerConfigElement
		: ConfigurationElement
	{
		[ConfigurationProperty ("name", IsRequired = true)]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}

		[ConfigurationProperty ("host", IsRequired = true)]
		public string Host
		{
			get { return (string)this["host"]; }
			set { this["host"] = value; }
		}

		[ConfigurationProperty ("port", IsRequired = true)]
		//[IntegerValidator (MinValue = 1, MaxValue = 32767, ExcludeRange = false)]
		public int Port
		{
			get { return (int)this["port"]; }
			set { this["port"] = value; }
		}

		[ConfigurationProperty ("type", IsRequired = true)]
		public string Type
		{
			get { return (string)this["type"]; }
			set { this["type"] = value; }
		}
	}
}