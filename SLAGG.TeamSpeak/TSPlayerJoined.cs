using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using System.Net.Sockets;
using System.Threading;
using System.Configuration;

namespace SLAGG.TeamSpeak
{
	public class TSPlayerJoined
		: IPlugin
	{
		#region IPlugin Members
		public bool IsPublic
		{
			get { return true; }
		}

		public PluginType Type
		{
			get { return PluginType.Listener; }
		}

		public bool IsRunning
		{
			get { return (this.ts != null); }
		}

		public void Start (IMessanger messanger)
		{
			this.messanger = messanger;

			this.ts = TeamSpeak.GetTeamspeak (ConfigurationManager.AppSettings["tsServer"], Int32.Parse (ConfigurationManager.AppSettings["tsQueryPort"]));
			this.ts.ErrorOccured	+= this.ts_ErrorOccured;
			this.ts.PlayerJoined	+= this.ts_PlayerJoined;
			this.ts.PlayerLeft		+= this.ts_PlayerLeft;
			this.ts.StartListener (15000);
		}

		public void Stop ()
		{
			this.ts.EndListener ();
			this.ts.PlayerJoined	-= this.ts_PlayerJoined;
			this.ts.PlayerLeft		-= this.ts_PlayerLeft;
			this.ts.ErrorOccured	-= this.ts_ErrorOccured;
			this.ts = null;

			this.messanger = null;
		}

		public void ProcessPublicMessage (string nick, string message)
		{
			throw new NotSupportedException ();
		}

		public void ProcessPrivateMessage (string nick, string message)
		{
			throw new NotSupportedException ();
		}
		#endregion

		private IMessanger messanger;
		private bool listening;

		private TeamSpeak ts;

		void ts_PlayerLeft (object sender, PlayerEventArgs e)
		{
			this.messanger.SendToChannel (e.Player.NickName + " has left TeamSpeak.");
		}

		void ts_PlayerJoined (object sender, PlayerEventArgs e)
		{
			this.messanger.SendToChannel (e.Player.NickName + " has joined TeamSpeak.");
		}

		void ts_ErrorOccured (object sender, ErrorEventArgs e)
		{
			this.messanger.SendToChannel ("SLAGG.TSPlayerJoined died a horrible death: " + e.Error.Message);
			this.Stop ();
		}
	}
}