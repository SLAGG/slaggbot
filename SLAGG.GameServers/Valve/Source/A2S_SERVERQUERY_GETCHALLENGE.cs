using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.GameServers.Valve.Source
{
	public class A2S_SERVERQUERY_GETCHALLENGE
	{
		public A2S_SERVERQUERY_GETCHALLENGE (byte[] msg)
		{
			this.FromMessage (msg);
		}

		public int Challenge
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
					byte[] header = BitConverter.GetBytes (-1);

					requestPacket = new byte[header.Length + 1];

					for (int i = 0; i < header.Length; ++i)
						requestPacket[i] = header[i];

					requestPacket[4] = 0x57;
				}

				return requestPacket;
			}
		}

		private static byte[] requestPacket;

		private void FromMessage (byte[] msg)
		{
			int i = 4;

			byte first = msg[i];

			if (first == (byte)PacketType.A2S_SERVERQUERY_GETCHALLENGE) // Skip first byte if you haven't already.
				++i;

			this.Challenge = ByteExtensions.GetInt32 (msg, i, ref i);
		}
	}
}