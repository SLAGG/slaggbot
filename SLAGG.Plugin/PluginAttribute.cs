using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.Plugin
{
	public class PluginAttribute
		: Attribute
	{
		public PluginAttribute (string usage)
		{
			this.Usage = usage;
		}

		public PluginAttribute (string usage, string description)
			: this (usage)
		{
			this.Description = description;
		}

		public string Usage
		{
			get;
			set;
		}
		
		public string Description
		{
			get;
			set;
		}
	}
}