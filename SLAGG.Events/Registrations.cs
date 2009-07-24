using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using MySql.Data.MySqlClient;
using Mono.Rocks;

namespace SLAGG.Events
{
	public class Registrations
		: ParserPluginBase
	{
		#region IPlugin Members
		public override void Stop ()
		{
			if (this.connection != null)
				this.connection.Close ();
			
			this.connection = null;

			base.Stop ();
		}

		#endregion

		private MySqlConnection connection;

		protected override string ProcessMessage (string nick, string message)
		{
			if (!message.StartsWith ("~nextevent"))
				return null;
			
			try
			{
				var ev = EventConnection.GetEvents().OrderBy (e => e.Date).FirstOrDefault();
				return (ev == null)
				       	? "No next event."
				       	: String.Format ("Next Event: {0} at {1}. Registered [{3}]: {2}", ev.Date.ToString ("ddd, MMMM d"),
				       	                 ev.Location, ev.Registered.Explode (","), ev.Registered.Count());
			}
			catch (MySqlException ex)
			{
				this.Stop();
				return "Registrations module b0rked: " + ex.Message;
			}
		}
	}
}