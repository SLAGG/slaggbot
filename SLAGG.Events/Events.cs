using System;
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
			var msgBuilder = new StringBuilder ();

			if (message.StartsWith ("~events"))
			{
				var connection = new MySqlConnection (EventConnections.ConnectionString);
				connection.Open ();

				var connection2 = new MySqlConnection (EventConnections.ConnectionString);
				connection2.Open();

				var cmd = connection.CreateCommand ();
				cmd.CommandText = "SELECT shortname,evid,evdatestart FROM locations,events WHERE (evdatestart>now() AND locations.locid=events.locid) ORDER BY evdatestart LIMIT 4;";

				var reader = cmd.ExecuteReader ();
				while (reader.Read ())
				{
					//if (msgBuilder.Length == 0)
					//    msgBuilder.Append ("Upcoming events: ");

					var cmd2 = connection2.CreateCommand ();
					cmd2.CommandText = "SELECT COUNT(*) FROM users,userhist WHERE (uid=id AND evid=" + Convert.ToUInt64 (reader["evid"]) + " AND tcreate>0)";
					var count = cmd2.ExecuteScalar ();
					cmd2.Dispose();

					msgBuilder.Append (String.Format ("{1} on {0}, {2} gamers registered. ", ((DateTime)reader["evdatestart"]).ToString ("ddd, MMMM d"), (string)reader["shortname"], count));
				}

				reader.Close ();
				cmd.Dispose ();

				connection.Dispose();
				connection2.Dispose();
			}

			return msgBuilder.ToString ();
		}
	}
}