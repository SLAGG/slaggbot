using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.GameServers
{
	public class Player
		: IPlayer
	{
		public string Name
		{
			get;
			set;
		}

		public int Score
		{
			get;
			set;
		}
	}
}