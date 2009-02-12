using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SLAGG.Plugin.Configuration
{
	public class ConfigCollection<T, TKey>
		: ConfigurationElementCollection, IEnumerable<T>
		where T : ConfigurationElement, IConfigElement<TKey>, new()
	{
		public T this[TKey key]
		{
			get { return (T)this.BaseGet (key); }
		}

		protected override ConfigurationElement CreateNewElement ()
		{
			return new T ();
		}

		protected override object GetElementKey (ConfigurationElement element)
		{
			return ((IConfigElement<TKey>)element).Key;
		}

		#region IEnumerable<T> Members

		public new IEnumerator<T> GetEnumerator ()
		{
			var enumerator = base.GetEnumerator ();
			enumerator.Reset ();

			while (enumerator.MoveNext ())
				yield return (T)enumerator.Current;
		}

		#endregion
	}
}