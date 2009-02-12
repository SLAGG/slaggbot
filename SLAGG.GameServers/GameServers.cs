using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using SLAGG.GameServers.Valve;
using System.Net.Sockets;
using System.IO;
using SLAGG.GameServers.Valve.Source;
using System.Text.RegularExpressions;
using System.Configuration;

namespace SLAGG.GameServers
{
	[Plugin ("~gs|~gameservers [server]", "Lists and queries SLAGG game servers")]
	public class GameServers
		: ParserPluginBase
	{
		#region IPlugin Members

		public override void Start (IMessanger messanger)
		{
			this.config = (GameServersSection)ConfigurationManager.GetSection ("gameservers");

			base.Start (messanger);
		}

		public override void Stop ()
		{
			this.config = null;

			base.Stop ();
		}

		protected override string ProcessMessage (string nick, string message)
		{
			var m = Regex.Match (message, @"~(gs|gameservers)\s*(?<server>.+)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			if (m.Success)
			{
				if (String.IsNullOrEmpty (m.Groups["server"].Value))
				{
					bool first = true;
					var servers = new StringBuilder ("Gameservers: ");
					foreach (var s in this.config.Servers)
					{
						if (first)
							first = false;
						else
							servers.Append (", ");

						servers.Append (s.Name);
						servers.Append (" [");
						servers.Append (s.Host);
						servers.Append (":");
						servers.Append (s.Port);
						servers.Append ("][");
						servers.Append (this.GetServer (s.Name).GetType ().Name);
						servers.Append ("]");
					}

					return servers.ToString ();
				}
				else
				{
					var server = this.GetServer (m.Groups["server"].Value);
					if (server != null)
					{
						try
						{
							if (!server.IsConnected)
								server.Connect ();

							IServerInfo serverInfo = server.GetServerInfo ();
							IEnumerable<IPlayer> players = server.GetPlayers ();

							var serverMessage = new StringBuilder (serverInfo.ServerName);
							serverMessage.Append (" playing ");
							serverMessage.Append (serverInfo.MapName);
							serverMessage.Append (" Players [");
							serverMessage.Append (serverInfo.NumPlayers);
							serverMessage.Append ("/");
							serverMessage.Append (serverInfo.MaxPlayers);
							serverMessage.Append ("]: ");

							bool first = true;
							foreach (var player in players)
							{
								if (first)
									first = false;
								else
									serverMessage.Append (", ");

								serverMessage.Append (player.Name);
							}

							return serverMessage.ToString ();
						}
						catch (GameServerException ex)
						{
							return "Querying '" + server.Host + "' failed: " + ex.Message;
						}
					}
				}
			}

			return null;
		}

		private IGameServer GetServer (string key)
		{
			var serverConfig = this.config.Servers[key];
			if (serverConfig != null)
			{
				if (this.servers == null)
					this.servers = new Dictionary<string, IGameServer> (this.config.Servers.Count);

				if (!this.servers.ContainsKey (key))
					this.servers.Add (key, (IGameServer)Activator.CreateInstance (System.Type.GetType (serverConfig.Type), serverConfig));

				return this.servers[key];
			}

			return null;
		}
		#endregion

		private Dictionary<string, IGameServer> servers;

		private GameServersSection config;
	}

	public class GameServerException
		: Exception
	{
		public GameServerException (Exception innerException)
			: base (innerException.Message, innerException) { }
	}
}