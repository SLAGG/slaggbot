using System;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using MySql.Data.MySqlClient;

namespace SLAGG.Events
{
	[Plugin ("~events", "Lists the next four SLAGG events")]
	public class Events
		: ParserPluginBase
	{
		protected override string ProcessMessage (string nick, string message)
		{
			if (!message.StartsWith ("~events"))
				return null;
			
			var msgBuilder = new StringBuilder ();
			foreach (var ev in EventConnection.GetEvents())
				msgBuilder.AppendFormat ("{1} on {0}, {2} gamers registered. ", ev.Date.ToString ("ddd, MMMM d"), ev.Location, ev.Registered.Count());

			return msgBuilder.ToString();
		}
	}
}