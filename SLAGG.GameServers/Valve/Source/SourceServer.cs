using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace SLAGG.GameServers.Valve.Source
{
	public class SourceServer
		: GameServerConfigElement, IGameServer
	{
		public SourceServer (GameServerConfigElement config)
		{
			this.Host = config.Host;
			this.Port = config.Port;
			this.Name = config.Name;
		}

		#region IGameServer Members
		public bool IsConnected
		{
			get
			{
				if (this.sock == null)
					return false;

				return this.sock.Connected;
			}
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
		}

		public IEnumerable<IPlayer> GetPlayers ()
		{
			try
			{
				this.sock.Send (A2S_SERVERQUERY_GETCHALLENGE.RequestPacket);

				byte[] challengeMsg = new byte[1500];
				this.sock.Receive (challengeMsg);

				var challenge = new A2S_SERVERQUERY_GETCHALLENGE (challengeMsg);

				this.sock.Send (A2S_PLAYER.GetRequestPacket (challenge.Challenge));

				byte[] playersMsg = new byte[1500];
				this.sock.Receive (playersMsg);
				this.sock.Close ();

				return (new A2S_PLAYER (playersMsg)).Players;
			}
			catch (SocketException ex)
			{
				throw new GameServerException (ex);
			}
		}

		public IServerInfo GetServerInfo ()
		{
			try
			{
				this.sock.Send (A2S_INFO.RequestPacket);

				byte[] buffer = new byte[1500];
				this.sock.Receive (buffer);

				return new A2S_INFO (buffer);
			}
			catch (SocketException ex)
			{
				throw new GameServerException (ex);
			}
		}

		#endregion

		private Socket sock;
	}

	public enum PacketType
		: byte
	{
		A2A_PING = 0x69,
		A2S_INFO = 0x49,
		A2S_PLAYER = 0x44,
		A2S_RULES = 0x45,
		A2S_SERVERQUERY_GETCHALLENGE = 0x41
	}
}