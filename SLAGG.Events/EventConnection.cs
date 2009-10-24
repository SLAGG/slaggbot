using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using MySql.Data.MySqlClient;

namespace SLAGG.Events
{
	internal static class EventConnection
	{
		public static string ConnectionString
		{
			get
			{
				lock (lck)
				{
					if (builder == null)
					{
						builder = new MySqlConnectionStringBuilder ();
						builder.Server = ConfigurationManager.AppSettings["dbServer"];
						builder.UserID = ConfigurationManager.AppSettings["dbUser"];
						builder.Password = ConfigurationManager.AppSettings["dbPassword"];
						builder.Database = ConfigurationManager.AppSettings["dbDatabase"];
					}

					return builder.ConnectionString;
				}
			}
		}

		public static IEnumerable<Event> GetEvents()
		{
			var connection = new MySqlConnection (ConnectionString);
			connection.Open ();

			var cmd = connection.CreateCommand ();
			cmd.CommandText = "SELECT shortname,evid,evdatestart FROM locations,events WHERE (evdatestart>now() AND locations.locid=events.locid) ORDER BY evdatestart LIMIT 5;";

			var connection2 = new MySqlConnection (ConnectionString);
			connection2.Open();

			var reader = cmd.ExecuteReader ();
			while (reader.Read ())
			{
				Event ev = new Event (Convert.ToInt32 (reader["evid"]), (string)reader["shortname"], (DateTime)reader["evdatestart"]);
				long eventId = Convert.ToInt64 (reader["evid"]);

				cmd = connection2.CreateCommand ();
				cmd.CommandText = "SELECT handle FROM users,userhist WHERE (uid=id AND evid=" + eventId + " AND tcreate>0)";

				int i = 0;

				List<string> registrants = new List<string>();
				reader = cmd.ExecuteReader ();
				while (reader.Read ())
				{
					++i;
					registrants.Add ((string)reader["handle"]);
				}

				ev.Registered = registrants;

				connection.Close ();
				connection.Dispose ();

				yield return ev;
			}

			reader.Close ();
			cmd.Dispose ();
		}

		private static object lck = new object ();

		private static MySqlConnectionStringBuilder builder;
	}
}