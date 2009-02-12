using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using System.Text.RegularExpressions;

namespace SLAGGBot
{
	[Plugin ("~help [module]", "Provides a description and usage of plugins")]
	public class Help
		: ParserPluginBase
	{
		private Regex help = new Regex (@"^~help\s*(?<module>[^\s]+)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		protected override string ProcessMessage (string nick, string message)
		{
			StringBuilder msgBuilder = new StringBuilder ();

			var m = help.Match (message);
			if (m.Success)
			{
				string module = m.Groups["module"].Value.ToLower();

				IPlugin plugin = (from p in SLAGGBot.Plugins
										  where (p.GetType ().Name.ToLower () == module || p.GetType ().FullName.ToLower () == module)
										  select p).FirstOrDefault ();

				if (plugin != null)
				{
					PluginAttribute[] pluginAttributes = (PluginAttribute[])plugin.GetType ().GetCustomAttributes (typeof (PluginAttribute), false);
					if (pluginAttributes.Length == 1)
					{
						msgBuilder.Append (plugin.GetType ().FullName);

						if (!String.IsNullOrEmpty (pluginAttributes[0].Description))
						{
							msgBuilder.Append ("  Description: ");
							msgBuilder.Append (pluginAttributes[0].Description);
						}

						if (!String.IsNullOrEmpty (pluginAttributes[0].Usage))
						{
							msgBuilder.Append ("  Usage: ");
							msgBuilder.Append (pluginAttributes[0].Usage);
						}
					}
					else
					{
						msgBuilder.Append ("Help for module '");
						msgBuilder.Append (plugin.GetType ().FullName);
						msgBuilder.Append ("' is unavailable.");
					}
				}
				else if (module.Trim() == String.Empty)
				{
					return ModulesPlugin.GetModules ();
				}
				else
				{
					msgBuilder.Append ("Module '");
					msgBuilder.Append (module);
					msgBuilder.Append ("' not found.");
				}
			}

			return msgBuilder.ToString ();
		}
	}
}