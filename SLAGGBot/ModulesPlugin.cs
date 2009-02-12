using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using System.Text.RegularExpressions;

namespace SLAGGBot
{
	[Plugin ("Don't touch me.")]
	public class ModulesPlugin
		: ParserPluginBase
	{
		public override bool IsPublic
		{
			get { return false; }
		}

		protected override string ProcessMessage (string nick, string message)
		{
			if (message.StartsWith ("~") && nick.IsOperator ())
			{
				if (message.StartsWith ("~modules"))
				{
					return GetModules ();
				}
				else if (message.StartsWith ("~quit"))
				{
					SLAGGBot.IRCConnection.Disconnect (nick + " wants me gone apparently.");
					Environment.Exit (0);
				}
				else if (message.StartsWith ("~kill"))
				{
					SLAGGBot.Messanger.PerformAction ("was shot by " + nick + ".");
					SLAGGBot.IRCConnection.Disconnect ("Shot dead.");

					Environment.Exit (1);
				}
				else
				{
					var mtch = Regex.Match (message, @"~(?<cmd>enable|disable)\s*(?<module>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
					string module = mtch.Groups["module"].Value.ToLower ();
					if (mtch.Success && module != "modulesplugin")
					{
						IPlugin plugin = (from p in SLAGGBot.Plugins
										  where (p.GetType ().Name.ToLower () == module || p.GetType ().FullName.ToLower () == module)
										  select p).FirstOrDefault ();
						if (plugin != null)
						{
							switch (mtch.Groups["cmd"].Value)
							{
								case "enable":
									{
										if (!plugin.IsRunning)
										{
											plugin.Start (SLAGGBot.Messanger);
											return plugin.GetType ().FullName + " enabled.";
										}

										break;
									}

								case "disable":
									{
										if (plugin.IsRunning)
										{
											plugin.Stop ();
											return plugin.GetType ().FullName + " disabled.";
										}

										break;
									}
							}
						}
					}
				}
			}

			return null;
		}

		internal static string GetModules ()
		{
			StringBuilder moduleBuilder = new StringBuilder ();

			foreach (var p in SLAGGBot.Plugins)
			{
				if (!p.IsPublic)
					continue;

				if (moduleBuilder.Length > 0)
					moduleBuilder.Append (", ");

				moduleBuilder.Append (p.GetType ().FullName);

				if (!p.IsRunning)
					moduleBuilder.Append (" [Disabled]");
			}

			return moduleBuilder.ToString ();
		}
	}
}
