using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace SLAGG.GameServers.iD
{
	public class Q3Server
		: GameServerConfigElement, IGameServer
	{
		public Q3Server (GameServerConfigElement config)
		{
			this.Host = config.Host;
			this.Port = config.Port;
			this.Name = config.Name;
		}

		#region IGameServer Members

		public bool IsConnected
		{
			get { return (this.sock != null); }
		}

		public void Connect ()
		{
			try
			{
				this.sock = new Socket (AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				this.sock.Connect (this.Host, this.Port);
			}
			catch (SocketException ex)
			{
				throw new GameServerException (ex);
			}
		}

		public void Disconnect ()
		{
			this.sock.Disconnect (false);
			this.sock = null;

			this.lastInfo = null;
			this.lastPlayers = null;
		}

		public IEnumerable<IPlayer> GetPlayers ()
		{
			this.QueryServer ();

			return this.lastPlayers;
		}

		public IServerInfo GetServerInfo ()
		{
			this.QueryServer ();
			
			return this.lastInfo;
		}

		#endregion

		private static Regex propertyMatcher = new Regex (@"\\(?<key>\w+)\\(?<value>[^\\]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static Regex playerMatcher = new Regex ("(?<score>\\d+)\\s(?<ping>\\d+)\\s\"(?<name>.+?)\"", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		private void QueryServer ()
		{
			try
			{
				if (this.lastInfo != null && (DateTime.Now.Subtract (this.lastQuery)).TotalSeconds < 5)
					return;

				this.lastQuery = DateTime.Now;

				byte[] header = BitConverter.GetBytes (-1);
				byte[] status = Encoding.ASCII.GetBytes ("getstatus");
				byte[] msg = new byte[header.Length + status.Length];
				for (int n = 0; n < header.Length; ++n)
					msg[n] = header[n];

				for (int n = 0; n < status.Length; ++n)
					msg[header.Length + n] = status[n];

				this.sock.Send (msg);

				byte[] buffer = new byte[1500];
				this.sock.Receive (buffer);

				int i = 4;
				string type = ByteExtensions.GetString (buffer, i, ref i, '\n');
				string results = ByteExtensions.GetString (buffer, i, ref i);

				var propMatches = propertyMatcher.Matches (results);
				var properties = new Dictionary<string, string> ();
				foreach (Match property in propMatches)
					properties.Add (property.Groups["key"].Value, property.Groups["value"].Value);

				var players = new List<IPlayer> ();

				var matches = playerMatcher.Matches (results);
				foreach (Match m in matches)
				{
					players.Add (new Player ()
					{
						Name = m.Groups["name"].Value,
						Score = int.Parse (m.Groups["score"].Value)
					});
				}
				//^0  black       // IRC 1,0
				//^1  red         // IRC 4
				//^2  green       // IRC 3
				//^3  yellow      // IRC 8
				//^4  blue        // IRC 2
				//^5  cyan        // IRC 11
				//^6  magenta     // IRC 6
				//^7  white       // IRC 0,1
				this.lastPlayers = players;

				this.lastInfo = new ServerInfo ()
				{
					ServerName = properties["sv_hostname"],
					MapName = properties["mapname"],
					NumPlayers = players.Count,
					MaxPlayers = int.Parse (properties["sv_maxclients"])
				};
			}
			catch (SocketException ex)
			{
				throw new GameServerException (ex);
			}
		}

		private DateTime lastQuery = DateTime.Today;
		private IServerInfo lastInfo;
		private IEnumerable<IPlayer> lastPlayers;

		private Socket sock;

		private static string GetString (byte[] buffer, int startIndex, ref int newStartIndex)
		{
			char b = (char)0x5C;

			var builder = new StringBuilder ();
			for (int i = startIndex; i < buffer.Length; ++i)
			{
				if (buffer[i] != b && buffer[i] != '\n' && buffer[i] != '\0')
					builder.Append ((char)buffer[i]);
				else
				{
					newStartIndex = ++i;
					break;
				}
			}

			return builder.ToString ();
		}
	}
}