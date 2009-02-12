using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using MySql.Data.MySqlClient;

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
			if (message.StartsWith ("~nextevent"))
			{
				try
				{
					this.connection = new MySqlConnection (EventConnections.ConnectionString);
					this.connection.Open ();

					ulong eventID = 0;
					string location = null;
					DateTime date = DateTime.Now;

					var cmd = connection.CreateCommand ();
					cmd.CommandText = "SELECT shortname,evid,evdatestart FROM locations,events WHERE (evdatestart>now() AND locations.locid=events.locid) ORDER BY evdatestart LIMIT 1;";

					var reader = cmd.ExecuteReader ();
					if (reader.Read ())
					{
						eventID = Convert.ToUInt64 (reader["evid"]);
						location = (string)reader["shortname"];
						date = (DateTime)reader["evdatestart"];
					}

					reader.Close ();
					cmd.Dispose ();

					if (eventID != 0)
					{
						StringBuilder registered = new StringBuilder ();

						cmd = connection.CreateCommand ();
						cmd.CommandText = "SELECT handle FROM users,userhist WHERE (uid=id AND evid=" + eventID + " AND tcreate>0)";

						int i = 0;

						reader = cmd.ExecuteReader ();
						while (reader.Read ())
						{
							++i;

							if (registered.Length > 0)
								registered.Append (", ");

							registered.Append ((string)reader["handle"]);
						}

						reader.Close ();
						cmd.Dispose ();

						this.connection.Close ();
						this.connection.Dispose ();
						this.connection = null;

						return String.Format ("Next Event: {0} at {1}. Registered [{3}]: {2}", date.ToString ("ddd, MMMM d"), location, registered.ToString (), i);
					}
				}
				catch (MySqlException ex)
				{
					this.Stop ();
					return "Registrations module b0rked: " + ex.Message;
				}
			}

			return null;
		}
	}
}