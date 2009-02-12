using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.Plugin
{
	public interface IMessanger
	{
		void PerformAction (string action);
		void SendToChannel (string message);
		void SendToUser (string user, string message);
	}
}