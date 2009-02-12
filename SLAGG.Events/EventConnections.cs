using System.Configuration;
using MySql.Data.MySqlClient;

namespace SLAGG.Events
{
	internal class EventConnections
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

		private static object lck = new object ();

		private static MySqlConnectionStringBuilder builder;
	}
}
