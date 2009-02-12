using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using MySql.Data.MySqlClient;
using System.Threading;

namespace SLAGG.Forums
{
	public class ForumPosts
		: IPlugin
	{
		#region IPlugin Members
		public bool IsPublic
		{
			get { return true; }
		}

		public PluginType Type
		{
			get { return PluginType.Listener; }
		}

		public bool IsRunning
		{
			get { return this.listening; }
		}

		public void Start (IMessanger messanger)
		{
			this.listening = true;
			this.messanger = messanger;

			try
			{
				this.connection = new MySqlConnection (Forums.ConnectionString);
				this.connection.Open ();
			}
			catch (MySqlException ex)
			{
				this.messanger.SendToChannel ("ForumPosts module b0rked: " + ex.Message);
				this.Stop ();

				return;
			}

			Thread listenerThread = new Thread (this.Listener)
			{
				Name = "Forum post listener",
				IsBackground = true
			};
			
			listenerThread.Start ();
		}

		public void Stop ()
		{
			this.listening = false;
		}

		public void ProcessPublicMessage (string nick, string message)
		{
			throw new NotSupportedException ();
		}

		public void ProcessPrivateMessage (string nick, string message)
		{
			throw new NotSupportedException ();
		}

		#endregion

		private IMessanger messanger;

		private MySqlConnection connection;
		private ulong lastChecked = DateTime.UtcNow.GetTimeT ();

		private bool listening;
		private ulong lastPostID;

		private void Listener ()
		{
			try
			{
				while (this.listening)
				{
					MySqlDataReader reader = null;
					MySqlCommand cmd = null;

					try
					{
						cmd = connection.CreateCommand ();
						cmd.CommandText = "SELECT post_id,topic_id,forum_id,poster_id FROM phpbb_posts WHERE (post_time>" + this.lastChecked + ")";
						reader = cmd.ExecuteReader ();

						ulong checkedTime = DateTime.UtcNow.GetTimeT ();

						while (reader.Read ())
						{
							if (Forums.GetForumVisible (Convert.ToUInt64 (reader["forum_id"])))
							{
								ulong postID = Convert.ToUInt64 (reader["post_id"]);

								if (postID != this.lastPostID)
								{
									ulong topicID = Convert.ToUInt64 (reader["topic_id"]);

									var msgBuilder = new StringBuilder (Forums.GetUsername (Convert.ToUInt64 (reader["poster_id"])));
									msgBuilder.Append (" posted in ");
									msgBuilder.Append (Forums.GetTopicSubject (topicID));
									msgBuilder.Append (" ( http://www.slagg.org/F0rUmZ/viewtopic.php?p=");
									msgBuilder.Append (postID);
									msgBuilder.Append ("#");
									msgBuilder.Append (postID);
									msgBuilder.Append (" )");

									this.messanger.SendToChannel (msgBuilder.ToString ());

									this.lastPostID = postID;
								}
							}
						}
						reader.Close ();
						cmd.Dispose ();

						this.lastChecked = checkedTime;

						Thread.Sleep (60000);
						//Thread.Sleep (5000);
					}
					catch (Exception ex)
					{
						this.messanger.SendToChannel ("ForumPosts module b0rked: " + ex.Message);
						this.Stop ();
					}
					finally
					{
						if (reader != null)
							reader.Close ();

						if (cmd != null)
							cmd.Dispose ();
					}
				}
			}
			finally
			{
				if (this.connection != null)
				{
					this.connection.Dispose ();
					this.connection = null;
				}
			}
		}
	}
}