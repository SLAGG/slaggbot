using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using System.Configuration;

namespace SLAGG.TeamSpeak
{
	public class TSPlayers
		: ParserPluginBase
	{
		#region IPlugin Members
		public override void Start (IMessanger messanger)
		{
			this.ts = TeamSpeak.GetTeamspeak (ConfigurationManager.AppSettings["tsServer"], Int32.Parse (ConfigurationManager.AppSettings["tsQueryPort"]));
			this.ts.ErrorOccured += this.ts_ErrorOccured;

			base.Start (messanger);
		}

		public override void Stop ()
		{
			this.ts.ErrorOccured -= this.ts_ErrorOccured;
			this.ts = null;

			base.Stop ();
		}
		#endregion

		private TeamSpeak ts;

		protected override string ProcessMessage (string nick, string message)
		{
			if (message.StartsWith ("~teamspeak"))
			{
				var msg = new StringBuilder ();
				var list = new StringBuilder ();

				var players = this.ts.GetPlayerList ();
				if (players != null)
				{
					if (players.Count () > 0)
					{
						foreach (var p in players)
						{
							if (list.Length > 0)
								list.Append (", ");

							list.Append (p.NickName);
						}
					}

					msg.Append ("TeamSpeak: ");
					msg.Append (ConfigurationManager.AppSettings["tsDisplayServer"]);
					
					msg.Append ("  Players [");
					msg.Append (players.Count ());
					msg.Append ("]");

					if (players.Count () > 0)
					{
						msg.Append (": ");
						msg.Append (list.ToString ());
					}

					return msg.ToString ();
				}
			}

			return null;
		}

		void ts_ErrorOccured (object sender, ErrorEventArgs e)
		{
			this.messanger.SendToChannel ("SLAGG.TSPlayers died a horrible death: " + e.Error.Message);
			this.Stop ();
		}
	}
}