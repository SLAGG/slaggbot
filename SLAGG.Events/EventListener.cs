using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SLAGG.Plugin;
using System.Linq;

namespace SLAGG.Events
{
	public class EventListener
		//: IPlugin
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
			get { return PluginType.Listener; }
		}

		/// <summary>
		/// Gets whether the plugin is currently running
		/// </summary>
		public bool IsRunning
		{
			get { return this.listening; }
		}

		/// <summary>
		/// Starts the plugin
		/// </summary>
		/// <param name="m">The messanger the plugin should use to communicate</param>
		public void Start (IMessanger m)
		{
			this.messanger = m;
			this.listening = true;

			(this.listenerThread = new Thread (this.Listener) { IsBackground = true }).Start();
		}

		/// <summary>
		/// Stops the plugin
		/// </summary>
		public void Stop()
		{
			this.listening = false;
			this.messanger = null;

			if (this.listenerThread != null)
				this.listenerThread.Join();
		}

		/// <summary>
		/// Processes messages sent to the channel
		/// </summary>
		/// <param name="nick">Who sent the message to the channel</param>
		/// <param name="message">The message that was sent to the channel</param>
		public void ProcessPublicMessage (string nick, string message)
		{
		}

		/// <summary>
		/// Processes messages sent privately to the bot
		/// </summary>
		/// <param name="nick">Who sent the message to the channel</param>
		/// <param name="message">The message that was sent to the bot</param>
		public void ProcessPrivateMessage(string nick, string message)
		{
		}

		#endregion

		private volatile bool listening;
		private Thread listenerThread;
		private IMessanger messanger;

		private void Listener()
		{
			HashSet<string> nextEventPlayers = new HashSet<string>();
			while (this.listening)
			{
				var nextEvent = EventConnection.GetEvents().OrderBy (e => e.Date).First();

				var unchanged = nextEventPlayers.Intersect (nextEvent.Registered);
				var joiningPlayers = nextEvent.Registered.Where (p => !unchanged.Contains (p));
				foreach (var p in joiningPlayers)
				{
					messanger.SendToChannel (p + " registered for the next event (" + nextEvent.Date.ToString ("ddd, MMMM d") + " at " + nextEvent.Location + ")");
				}

				var leavingPlayers = nextEventPlayers.Where (p => !unchanged.Contains (p));
				foreach (var p in leavingPlayers)
				{
					messanger.SendToChannel (p + " unregistered for the next event (" + nextEvent.Date.ToString ("ddd, MMMM d") + " at " + nextEvent.Location + ")");
				}

				nextEventPlayers = new HashSet<string> (nextEvent.Registered);		

				Thread.Sleep (180000);
			}
		}
	}
}