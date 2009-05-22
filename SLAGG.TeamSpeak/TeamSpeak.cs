using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;

namespace SLAGG.TeamSpeak
{
	public class TeamSpeak
	{
		public TeamSpeak (string host, int port)
		{
			this.Host = host;
			this.Port = port;
		}

		public event EventHandler<PlayerEventArgs> PlayerJoined = delegate { };
		public event EventHandler<PlayerEventArgs> PlayerLeft = delegate { };
		public event EventHandler<ErrorEventArgs> ErrorOccured = delegate { };

		public string Host
		{
			get;
			set;
		}

		public int Port
		{
			get;
			set;
		}

		public IEnumerable<TeamSpeakPlayer> GetPlayerList ()
		{
			this.Query ();

			return this.players;
		}

		public void StartListener (int frequency)
		{
			this.frequency = frequency;
			this.listening = true;

			Thread listenerThread = new Thread (this.Listener)
			{
				Name = "TeamSpeak Listener",
				IsBackground = true
			};

			listenerThread.Start ();
		}

		public void Query ()
		{
			byte[] buffer;

			try
			{
				if (!this.Socket.Connected)
				{
					this.Socket.Connect (this.Host, this.Port);

					// Get connection message
					buffer = new byte[4096];
					this.Socket.Receive (buffer);

					// Select server and get response
					this.Socket.Send (Encoding.ASCII.GetBytes ("sel 8767" + Environment.NewLine));
					buffer = new byte[4096];
					this.Socket.Receive (buffer);
				}

				// Request player list
				byte[] requestBytes = Encoding.ASCII.GetBytes ("pl" + Environment.NewLine);
				this.Socket.Send (requestBytes);

				buffer = new byte[4096];
				this.Socket.Receive (buffer);

				var currentPlayers = new HashSet<TeamSpeakPlayer> ();

				string[] players = Encoding.ASCII.GetString (buffer).Split (Environment.NewLine.ToCharArray ());
				for (int i = 2; i < players.Length; i += 2)
				{
					if (players[i] != "OK" && players[i][0] != '\0')
					{
						string[] columns = players[i].Split ('\t');
						string nickName = columns[14].Replace ("\"", String.Empty);
						if (ConfigurationManager.AppSettings["tsNickname"] == nickName)
							continue;

						string registeredName = columns[15].Replace ("\"", String.Empty);

						currentPlayers.Add (new TeamSpeakPlayer
											{
												NickName = nickName,
												RegisteredName = registeredName
											});
					}
				}

				var unchanged = this.players.Intersect (currentPlayers);

				var joiningPlayers = currentPlayers.Where (p => !unchanged.Contains (p));
				foreach (var p in joiningPlayers)
					this.PlayerJoined (this, new PlayerEventArgs (p));

				var leavingPlayers = this.players.Where (p => !unchanged.Contains (p));
				foreach (var p in leavingPlayers)
					 this.PlayerLeft (this, new PlayerEventArgs (p));

				this.players = currentPlayers;
			}
			catch (Exception ex)
			{
				this.players = null;
				this.ErrorOccured (this, new ErrorEventArgs (ex));
			}
		}

		public void EndListener ()
		{
			this.listening = false;

			this.Socket.Close ();
			this.Socket = null;
		}

		private Socket sock;
		private bool listening;
		private int frequency;

		private HashSet<TeamSpeakPlayer> players = new HashSet<TeamSpeakPlayer> ();

		protected Socket Socket
		{
			get
			{
				if (this.sock == null)
					this.sock = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				return this.sock;
			}

			set { this.sock = value; }
		}

		private void Listener ()
		{
			while (this.listening)
			{
				this.Query ();
				Thread.Sleep (this.frequency);
			}
		}

		#region Statics
		public static TeamSpeak GetTeamspeak (string host, int port)
		{
			lock (lck)
			{
				string key = host + ":" + port;
				if (!tses.ContainsKey (key))
					tses.Add (key, new TeamSpeak (host, port));
				
				return tses[key];
			}
		}

		private static object lck = new object ();
		private static Dictionary<string, TeamSpeak> tses = new Dictionary<string, TeamSpeak> ();
		#endregion
	}
}