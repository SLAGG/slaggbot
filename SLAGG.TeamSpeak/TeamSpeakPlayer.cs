using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.TeamSpeak
{
	public class TeamSpeakPlayer
	{
		public string RegisteredName
		{
			get;
			set;
		}

		public string NickName
		{
			get;
			set;
		}

		public override bool Equals (object obj)
		{
			if (this == null || obj == null)
				return false;

			var player = obj as TeamSpeakPlayer;
			if (player == null)
				return false;
			else if (!String.IsNullOrEmpty (player.RegisteredName) && !String.IsNullOrEmpty (this.RegisteredName))
				return (player.RegisteredName == this.RegisteredName);
			else
				return (player.NickName == this.NickName);
		}

		public override int GetHashCode ()
		{
			if (!String.IsNullOrEmpty (this.RegisteredName))
				return this.RegisteredName.GetHashCode ();
			else
				return this.NickName.GetHashCode ();
		}
	}
}