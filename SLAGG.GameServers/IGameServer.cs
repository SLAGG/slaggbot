using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.GameServers
{
	public interface IGameServer
	{
		bool IsConnected { get; }
		string Host { get; set; }
		int Port { get; set; }

		void Connect ();
		void Disconnect ();

		IEnumerable<IPlayer> GetPlayers ();
		IServerInfo GetServerInfo ();
	}
}