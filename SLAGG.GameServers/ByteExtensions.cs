using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.GameServers
{
	public static class ByteExtensions
	{
		public static short GetShort (byte[] buffer, int startIndex, ref int newStartIndex)
		{
			newStartIndex += 2;
			return BitConverter.ToInt16 (buffer, startIndex);
		}

		public static string GetString (byte[] buffer, int startIndex, ref int newStartIndex)
		{
			return GetString (buffer, startIndex, ref newStartIndex, '\0');
		}
		
		public static string GetString (byte[] buffer, int startIndex, ref int newStartIndex, char end)
		{
			var builder = new StringBuilder ();
			for (int i = startIndex; i < buffer.Length; ++i)
			{
				if (buffer[i] != end)
					builder.Append ((char)buffer[i]);
				else
				{
					newStartIndex = ++i;
					break;
				}
			}

			return builder.ToString ();
		}

		public static int GetInt32 (byte[] buffer, int startIndex, ref int newStartIndex)
		{
			newStartIndex += 4;
			return BitConverter.ToInt32 (buffer, startIndex);
		}

		public static float GetSingle (byte[] buffer, int startIndex, ref int newStartIndex)
		{
			newStartIndex += 4;
			return BitConverter.ToSingle (buffer, startIndex);
		}
	}
}
