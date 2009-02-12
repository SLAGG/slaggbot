using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.GameServers.Valve.Source
{
	public class A2S_PLAYER
	{
		public A2S_PLAYER ()
		{
		}

		public A2S_PLAYER (byte[] msg)
		{
			this.FromMessage (msg);
		}

		public byte NumPlayers
		{
			get;
			set;
		}

		public IEnumerable<IPlayer> Players
		{
			get
			{
				if (this.players == null)
					this.players = new List<IPlayer> ();

				return this.players;
			}
		}

		private List<IPlayer> players;

		private void FromMessage (byte[] msg)
		{
			int i = 4;

			byte first = msg[i];

			if (first == (byte)PacketType.A2S_PLAYER) // Skip first byte if you haven't already.
				first = msg[++i];

			this.NumPlayers = first;

			this.players = new List<IPlayer> (this.NumPlayers);

			for (int p = 0; p < this.NumPlayers; ++p)
			{
				var player = new SourcePlayer();
				player.Index = msg[++i];
				player.Name = ByteExtensions.GetString (msg, ++i, ref i);
				player.Score = ByteExtensions.GetInt32 (msg, i, ref i);
				player.TimeConnected = ByteExtensions.GetSingle (msg, i, ref i);

				this.players.Add (player);
			}
		}

		public static byte[] GetRequestPacket (int challenge)
		{
			byte[] requestPacket = new byte[9];

			byte[] type = BitConverter.GetBytes (-1);
			for (int i = 0; i < type.Length; ++i)
				requestPacket[i] = type[i];

			requestPacket[4] = 0x55;

			byte[] chall = BitConverter.GetBytes (challenge);
			for (int i = 0; i < chall.Length; ++i)
				requestPacket[1 + type.Length + i] = chall[i];

			return requestPacket;
		}
	}

	public class SourcePlayer
		: Player, IPlayer
	{
		public byte Index
		{
			get;
			set;
		}

		public float TimeConnected
		{
			get;
			set;
		}
	}
}