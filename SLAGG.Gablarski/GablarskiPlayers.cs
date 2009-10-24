using System;
using System.Configuration;
using Gablarski.Client;
using Gablarski.Network;
using SLAGG.Plugin;
using System.Linq;

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
			this.gablarski = new GablarskiClient (new NetworkClientConnection());
			this.gablarski.Connected += new EventHandler (OnConnected);
			this.gablarski.ConnectionRejected += new EventHandler<RejectedConnectionEventArgs> (OnConnectionRejected);
			this.gablarski.Disconnected += new EventHandler (OnDisconnected);
			this.gablarski.Users.UserJoined += OnUserLoggedIn;
			this.gablarski.Users.UserDisconnected += OnUserDisconnected;
			this.gablarski.Connect (ConfigurationManager.AppSettings["gbServer"], Int32.Parse (ConfigurationManager.AppSettings["gbPort"]));
		}

		/// <summary>
		/// Stops the plugin
		/// </summary>
		public void Stop()
		{
			this.messanger = null;
			this.gablarski.Users.UserJoined -= OnUserLoggedIn;
			this.gablarski.Users.UserDisconnected -= OnUserDisconnected;
			this.gablarski.Disconnect();
			this.gablarski = null;
		}

		/// <summary>
		/// Processes messages sent to the channel
		/// </summary>
		/// <param name="nick">Who sent the message to the channel</param>
		/// <param name="message">The message that was sent to the channel</param>
		public void ProcessPublicMessage (string nick, string message)
		{
			if (!message.StartsWith ("~gablarski") || !message.StartsWith ("~gb"))
				return;

			var users = this.gablarski.Users.ToList();
			messanger.SendToChannel ("Gablarski: " + ConfigurationManager.AppSettings["gbDisplayServer"] + "  Players [" +
									users.Count + "]:" + users.Select (cu => cu.Nickname).Explode (","));
		}

		/// <summary>
		/// Processes messages sent privately to the bot
		/// </summary>
		/// <param name="nick">Who sent the message to the channel</param>
		/// <param name="message">The message that was sent to the bot</param>
		public void ProcessPrivateMessage(string nick, string message)
		{
			if (!message.StartsWith ("~gablarski") || !message.StartsWith ("~gb"))
				return;

			var users = this.gablarski.Users.ToList();
			messanger.SendToUser (nick, "Gablarski: " + ConfigurationManager.AppSettings["gbDisplayServer"] + "  Players [" +
										users.Count + "]: " + users.Select (cu => cu.Nickname).Explode (","));
		}

		#endregion

		private IMessanger messanger;
		private GablarskiClient gablarski;

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
			if (messanger != null)
				messanger.SendToChannel ("Connected to Gablarski server");
		}

		private void OnConnectionRejected (object sender, RejectedConnectionEventArgs e)
		{
			if (messanger != null)
				messanger.SendToChannel ("Gablarski connection rejected: " + e.Reason);

			Stop ();
		}

		private void OnDisconnected (object sender, EventArgs e)
		{
			Stop ();

			if (messanger != null)
			{
				messanger.SendToChannel ("Gablarski disconnected");
				Start (messanger);
			}
		}
	}
}