using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.GameServers
{
	public interface IPlayer
	{
		string Name { get; }
		int Score { get; }
	}
}