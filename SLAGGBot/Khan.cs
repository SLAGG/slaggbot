using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using System.Text.RegularExpressions;
using System.Threading;

namespace SLAGGBot
{
	public class Kahn
		: ParserPluginBase
	{
		//public override void SendHelp (string nick)
		//{
		//    this.messanger.SendToUser (nick, "~dococ [n]");
		//}

		//public override void SendHelp (string nick, string command)
		//{
		//    this.SendHelp (nick);
		//}

		protected override string ProcessMessage (string nick, string message)
		{
			var m = Regex.Match (message, @"~k(ahn|han|irk)\s*(?<length>\d+)?", RegexOptions.Compiled | RegexOptions.IgnoreCase);
			if (m.Success)
			{
				var dococ = new StringBuilder ("KH");

				int len = 32;
				if (!String.IsNullOrEmpty (m.Groups["length"].Value))
				{
					if (!int.TryParse (m.Groups["length"].Value, out len))
					    len = 32;
				}

				len = len.Trim (1000);

				for (int i = 0; i < len; ++i)
				{
					dococ.Append ("A");

					if (i == 200 && len > 200)
					{
						i = 0;
						len -= 200;
						this.messanger.SendToChannel (dococ.ToString ());
						this.messanger.PerformAction ("gasps for air.");
						dococ = new StringBuilder ();

						Thread.Sleep (1000);
					}
				}

				dococ.Append ("N!");

				this.messanger.SendToChannel (dococ.ToString ());
			}

			return null;
		}
	}
}
