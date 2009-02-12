using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace SLAGG.Forums
{
	internal static class Forums
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

		public static string GetUsername (ulong userID)
		{
			var connection = Forums.GetConnection ();

			lock (lck)
			{
				if (users == null)
					users = new Dictionary<ulong, string> ();

				if (!users.ContainsKey (userID))
				{
					connection.Open ();

					var cmd = connection.CreateCommand ();
					cmd.CommandText = "SELECT username FROM phpbb_users WHERE (user_id=" + userID + ")";

					var reader = cmd.ExecuteReader ();
					reader.Read();

					users.Add (userID, (string)reader["username"]);

					reader.Close ();
					cmd.Dispose ();

					connection.Close ();
				}

				return users[userID];
			}
		}

		public static bool GetForumVisible (ulong forumID)
		{
			Forums.LoadForums ();

			lock (lck)
			{
				return forums.ContainsKey (forumID);
			}
		}

		public static string GetPostSubject (ulong postID)
		{
			var connection = Forums.GetConnection ();

			lock (lck)
			{
				connection.Open ();

				var cmd = connection.CreateCommand ();
				cmd.CommandText = "SELECT post_subject FROM phpbb_posts_text WHERE (post_id=" + postID + ")";

				var reader = cmd.ExecuteReader ();
				reader.Read ();

				string subject = (string)reader["post_subject"];

				reader.Close ();
				cmd.Dispose ();

				connection.Close ();

				return subject;
			}
		}

		public static string GetTopicSubject (ulong topicID)
		{
			var connection = Forums.GetConnection ();

			lock (lck)
			{
				connection.Open ();

				var cmd = connection.CreateCommand ();
				cmd.CommandText = "SELECT topic_title FROM phpbb_topics WHERE (topic_id=" + topicID + ")";

				var reader = cmd.ExecuteReader ();
				reader.Read ();

				string subject = (string)reader["topic_title"];

				reader.Close ();
				cmd.Dispose ();

				connection.Close ();

				return subject;
			}
		}

		private static object lck = new object ();
		
		private static Dictionary<ulong, string> forums;
		private static Dictionary<ulong, string> users;

		private static MySqlConnectionStringBuilder builder;
		private static MySqlConnection connection;

		private static void LoadForums ()
		{
			var connection = Forums.GetConnection ();

			lock (lck)
			{
				connection.Open ();

				if (forums == null)
				{
					forums = new Dictionary<ulong, string> ();

					var cmd = connection.CreateCommand ();
					cmd.CommandText = "SELECT forum_name,forum_id FROM phpbb_forums WHERE (auth_read = 0)";

					var reader = cmd.ExecuteReader ();
					while (reader.Read ())
						forums.Add (Convert.ToUInt64 (reader["forum_id"]), (string)reader["forum_name"]);

					reader.Close ();
					cmd.Dispose ();
				}

				connection.Close ();
			}
		}

		private static MySqlConnection GetConnection()
		{
			string connStr = Forums.ConnectionString;

			lock (lck)
			{
				if (connection == null)
					connection = new MySqlConnection (connStr);

				//if (connection.State != System.Data.ConnectionState.Open)
				//    connection.Open ();
			}

			return connection;
		}
	}
}