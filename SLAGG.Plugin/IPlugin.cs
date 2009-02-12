using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.Plugin
{
	[Flags]
	public enum PluginType
	{
		/// <summary>
		/// Starts it's own listener for events to notify the channel.
		/// </summary>
		Listener = 1,

		/// <summary>
		/// Parses public channel messages
		/// </summary>
		Parser = 2,

		/// <summary>
		/// Parses private messages
		/// </summary>
		PrivateParser = 4,

		All = Listener | Parser | PrivateParser
	}

	public interface IPlugin
	{
		/// <summary>
		/// Gets whether the plugin is publicly visible plugin in ~modules
		/// </summary>
		bool IsPublic { get; }

		/// <summary>
		/// Gets the type of the plugin
		/// </summary>
		PluginType Type { get; }

		/// <summary>
		/// Gets whether the plugin is currently running
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Starts the plugin
		/// </summary>
		/// <param name="messanger">The messanger the plugin should use to communicate</param>
		void Start (IMessanger messanger);

		/// <summary>
		/// Stops the plugin
		/// </summary>
		void Stop ();

		/// <summary>
		/// Processes messages sent to the channel
		/// </summary>
		/// <param name="nick">Who sent the message to the channel</param>
		/// <param name="message">The message that was sent to the channel</param>
		void ProcessPublicMessage (string nick, string message);

		/// <summary>
		/// Processes messages sent privately to the bot
		/// </summary>
		/// <param name="nick">Who sent the message to the channel</param>
		/// <param name="message">The message that was sent to the bot</param>
		void ProcessPrivateMessage (string nick, string message);
	}
}