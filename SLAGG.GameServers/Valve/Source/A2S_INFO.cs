using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.GameServers.Valve.Source
{
	public class A2S_INFO
		: ServerInfo, IServerInfo
	{
		public A2S_INFO ()
		{
		}

		public A2S_INFO (byte[] msg)
		{
			this.FromMessage (msg);
		}

		public byte Version
		{
			get;
			set;
		}

		public string GameDirectory
		{
			get;
			set;
		}

		public string GameDescription
		{
			get;
			set;
		}

		public SteamApplication Application
		{
			get;
			set;
		}

		public byte NumBots
		{
			get;
			set;
		}

		public Dedicated Dedicated
		{
			get;
			set;
		}

		public HostOS HostOS
		{
			get;
			set;
		}

		public bool IsPassworded
		{
			get;
			set;
		}

		public bool IsSecure
		{
			get;
			set;
		}

		public string GameVersion
		{
			get;
			set;
		}

		public short GameServerPort
		{
			get;
			set;
		}

		public short SpectatorPort
		{
			get;
			set;
		}

		public string SpectatorServerName
		{
			get;
			set;
		}

		public string GameTagData
		{
			get;
			set;
		}

		public static byte[] RequestPacket
		{
			get
			{
				if (requestPacket == null)
				{
					byte[] type = BitConverter.GetBytes (-1);
					byte[] query = Encoding.ASCII.GetBytes ("Source Engine Query\0");

					requestPacket = new byte[type.Length + 1 + query.Length];

					for (int i = 0; i < type.Length; ++i)
						requestPacket[i] = type[i];

					requestPacket[4] = (byte)'T';

					for (int i = 0; i < query.Length; ++i)
						requestPacket[i + type.Length + 1] = query[i];
				}

				return requestPacket;
			}
		}

		private static byte[] requestPacket;

		[Flags]
		private enum ExtraDataFlag
		{
			GameTagData = 0x20,
			Spectator = 0x40,
			GamePort = 0x80,
		}

		private void FromMessage (byte[] msg)
		{
			int i = 4;

			byte first = msg[i];

			if (first == (byte)PacketType.A2S_INFO) // Skip first byte if you haven't already.
				first = msg[++i];

			this.Version = first;

			this.ServerName = ByteExtensions.GetString (msg, ++i, ref i);
			this.MapName = ByteExtensions.GetString (msg, i, ref i);
			this.GameDirectory = ByteExtensions.GetString (msg, i, ref i);
			this.GameDescription = ByteExtensions.GetString (msg, i, ref i);

			this.Application = (SteamApplication)ByteExtensions.GetShort (msg, i, ref i);

			this.NumPlayers = msg[i];
			this.MaxPlayers = msg[++i];
			this.NumBots = msg[++i];
			this.Dedicated = (Dedicated)msg[++i];
			this.HostOS = (HostOS)msg[++i];
			this.IsPassworded = (msg[++i] == 0x01);
			this.IsSecure = (msg[++i] == 0x01);

			this.GameVersion = ByteExtensions.GetString (msg, i, ref i);

			var edf = (ExtraDataFlag)msg[i];
			if ((edf & ExtraDataFlag.GamePort) == ExtraDataFlag.GamePort)
				this.GameServerPort = ByteExtensions.GetShort (msg, i, ref i);

			if ((edf & ExtraDataFlag.Spectator) == ExtraDataFlag.Spectator)
			{
				this.SpectatorPort = ByteExtensions.GetShort (msg, i, ref i);
				this.SpectatorServerName = ByteExtensions.GetString (msg, i, ref i);
			}

			if ((edf & ExtraDataFlag.GameTagData) == ExtraDataFlag.GameTagData)
				this.GameTagData = ByteExtensions.GetString (msg, i, ref i);
		}
	}

	public enum Dedicated
		: byte
	{
		Listen		= (byte)'l',
		Dedicated	= (byte)'d',
		SourceTV	= (byte)'p'
	}

	public enum HostOS
		: byte
	{
		Linux	= (byte)'l',
		Windows	= (byte)'w'
	}
}