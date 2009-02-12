using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.TeamSpeak
{
	public class PlayerEventArgs
		: EventArgs
	{
		public PlayerEventArgs (TeamSpeakPlayer player)
		{
			this.player = player;
		}

		public TeamSpeakPlayer Player
		{
			get { return this.player; }
		}

		private readonly TeamSpeakPlayer player;
	}

	public class ErrorEventArgs
		: EventArgs
	{
		public ErrorEventArgs (Exception ex)
		{
			this.Error = ex;
		}

		public Exception Error
		{
			get;
			private set;
		}
	}
}
