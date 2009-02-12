using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using Sharkbite.Irc;
using System.Configuration;

namespace SLAGGBot
{
	public class IRCMessanger
		: IMessanger
	{
		public IRCMessanger (Sender sender, string channel)
		{
			this.sender = sender;
			this.channel = channel;
		}

		#region IMessanger Members

		public void PerformAction (string action)
		{
			if (!String.IsNullOrEmpty (action))
			{
				this.sender.Action (this.channel, action);
				Console.ForegroundColor = ConsoleColor.DarkGreen;
				Console.WriteLine ("[{0}]: {1}", channel, ConfigurationManager.AppSettings["ircNick"] + " " + action);
			}
		}

		public void SendToChannel (string message)
		{
			if (!String.IsNullOrEmpty (message))
			{
				this.sender.PublicMessage (channel, message);
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine ("[{0}]: {1}", channel, message);
			}
		}

		public void SendToUser (string user, string message)
		{
			if (!String.IsNullOrEmpty (message))
			{
				this.sender.PrivateMessage (user, message);
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine ("[{0}]: {1}", user, message);
			}
		}

		#endregion

		private readonly Sender sender;
		private readonly string channel;
	}
}
