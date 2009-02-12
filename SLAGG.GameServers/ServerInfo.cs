using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.GameServers
{
	public class ServerInfo
		: IServerInfo
	{
		#region IServerInfo Members

		public string ServerName
		{
			get;
			set;
		}

		public int NumPlayers
		{
			get;
			set;
		}

		public int MaxPlayers
		{
			get;
			set;
		}

		public string MapName
		{
			get;
			set;
		}

		#endregion
	}
}