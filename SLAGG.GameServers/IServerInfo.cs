using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.GameServers
{
	public interface IServerInfo
	{
		string ServerName { get; }

		int NumPlayers { get; }
		int MaxPlayers { get; }

		string MapName { get; }
	}
}
