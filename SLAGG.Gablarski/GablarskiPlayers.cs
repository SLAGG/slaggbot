using System;
using System.Configuration;
using System.Text;
using Gablarski.Client;
using Gablarski.Network;
using SLAGG.Plugin;
using System.Linq;
using System.Threading;

namespace SLAGG.Gablarski
{
	public class GablarskiPlayers
		: IPlugin
	{
		#region Implementation of IPlugin

		/// <summary>
		/// Gets whether the plugin is publicly visible plugin in ~modules
		/// </summary>
		public bool IsPublic
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the type of the plugin
		/// </summary>
		public PluginType Type
		{
			get { return PluginType.Listener | PluginType.Parser; }
		}

		/// <summary>
		/// Gets whether the plugin is currently running
		/// </summary>
		public bool IsRunning
		{
			get { return (messanger != null); }
		}

		/// <summary>
		/// Starts the plugin
		/// </summary>
		/// <param name="m">The messanger the plugin should use to communicate</param>
		public void Start (IMessanger m)
		{
			this.messanger = m;
			StartGablarski ();
		}

		/// <summary>
		/// Stops the plugin
		/// </summary>
		public void Stop()
		{
			this.messanger = null;
			StopGablarski();
		}

		/// <summary>
		/// Processes messages sent to the channel
		/// </summary>
		/// <param name="nick">Who sent the message to the channel</param>
		/// <param name="message">The message that was sent to the channel</param>
		public void ProcessPublicMessage (string nick, string message)
		{
			if ((!message.StartsWith ("~gablarski") && !message.StartsWith ("~gb")) || this.gablarski == null)
				return;

			messanger.SendToChannel (GetUsersString());
		}

		/// <summary>
		/// Processes messages sent privately to the bot
		/// </summary>
		/// <param name="nick">Who sent the message to the channel</param>
		/// <param name="message">The message that was sent to the bot</param>
		public void ProcessPrivateMessage(string nick, string message)
		{
			if ((!message.StartsWith ("~gablarski") && !message.StartsWith ("~gb")) || this.gablarski == null)
				return;

			messanger.SendToUser (nick, GetUsersString());
		}

		#endregion

		private bool justStarting = true;
		private IMessanger messanger;
		private GablarskiClient gablarski;

		private string GetUsersString()
		{
			if (messanger == null || gablarski == null)
				return String.Empty;

			var users = this.gablarski.Users.ToList();

			StringBuilder builder = new StringBuilder ("Gablarski: ");
			builder.Append (ConfigurationManager.AppSettings["gbDisplayServer"]);
			builder.AppendFormat (" Players [{0}]: ", users.Count);

			foreach (var channel in users.GroupBy (u => u.CurrentChannelId))
			{
				builder.AppendFormat ("{0}: ", this.gablarski.Channels[channel.Key].Name);

				int i = 0;
				int count = channel.Count();
				foreach (var u in channel)
				{
					builder.Append (u.Nickname);
					
					if (i++ < count-1)
						builder.Append (", ");
				}

				builder.Append ("; ");
			}

			return builder.ToString();
		}

		private void OnUserLoggedIn (object sender, UserEventArgs e)
		{
			this.messanger.SendToChannel (e.User.Nickname + " has joined Gablarski.");
		}

		private void OnUserDisconnected (object sender, UserEventArgs e)
		{
			this.messanger.SendToChannel (e.User.Nickname + " has left Gablarski.");
		}

		private void OnConnected (object sender, EventArgs e)
		{
			if (messanger != null && !justStarting)
				messanger.SendToChannel ("Connected to Gablarski server");
		}

		private void OnConnectionRejected (object sender, RejectedConnectionEventArgs e)
		{
			if (messanger != null && justStarting)
				messanger.SendToChannel ("Gablarski connection rejected: " + e.Reason);
		}

		private void OnDisconnected (object sender, EventArgs e)
		{
			if (messanger != null && justStarting)
				messanger.SendToChannel ("Gablarski disconnected");
		}

		private void StopGablarski()
		{
			justStarting = false;

			this.gablarski.Users.UserJoined -= OnUserLoggedIn;
			this.gablarski.Users.UserDisconnected -= OnUserDisconnected;
			this.gablarski.Disconnect();
			this.gablarski = null;
		}

		private void StartGablarski ()
		{
			this.gablarski = new GablarskiClient (new NetworkClientConnection ());
			this.gablarski.Connected += OnConnected;
			this.gablarski.ConnectionRejected += OnConnectionRejected;
			this.gablarski.Disconnected += OnDisconnected;
			this.gablarski.Users.UserJoined += OnUserLoggedIn;
			this.gablarski.Users.UserDisconnected += OnUserDisconnected;
			this.gablarski.Connect (ConfigurationManager.AppSettings["gbServer"], Int32.Parse (ConfigurationManager.AppSettings["gbPort"]));
		}
	}
}