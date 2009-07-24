using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sharkbite.Irc;
using System.Reflection;
using System.IO;
using SLAGG.Plugin;
using System.Text.RegularExpressions;
using System.Configuration;
using System.Security.Permissions;
using System.Diagnostics;

namespace SLAGGBot
{
	internal static class SLAGGBot
	{
		public static bool IsOperator (this string nick)
		{
			return Operators.Contains (nick);
		}

		internal static readonly List<IPlugin> Plugins = new List<IPlugin> ();
		internal static readonly HashSet<string> Operators = new HashSet<string> ();

		internal static IRCMessanger Messanger;

		internal static Connection IRCConnection;

		static void Main (string[] args)
		{
			if (!Debugger.IsAttached)
				AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			//Console.WindowWidth = Console.LargestWindowWidth - 20;
			//Cosole.WindowHeight = Console.LargestWindowHeight - 20;
			Console.BufferHeight = 1000;

			string[] files = Directory.GetFiles (Environment.CurrentDirectory);

			IEnumerable<FileInfo> assemblies = (from file in files
												where (file.Contains ("SLAGG.") && file.EndsWith (".dll") && file != "SLAGG.Plugin.dll")
												select new FileInfo (file));

			foreach (var assembly in assemblies)
				SLAGGBot.LoadPlugins (Assembly.Load (assembly.Name.Substring (0, assembly.Name.Length - assembly.Extension.Length)));

			SLAGGBot.LoadPlugins (Assembly.GetEntryAssembly ());

			IRCConnection = new Connection (new ConnectionArgs (ConfigurationManager.AppSettings["ircNick"], ConfigurationManager.AppSettings["ircServer"]), false, false);
			IRCConnection.Listener.OnPublic			+= Listener_OnPublic;
			IRCConnection.Listener.OnPrivate		+= Listener_OnPrivate;
			IRCConnection.Listener.OnJoin			+= Listener_OnJoin;
			IRCConnection.Listener.OnDisconnected	+= Listener_OnDisconnected;
			IRCConnection.Listener.OnError			+= Listener_OnError;
			IRCConnection.Listener.OnNick			+= Listener_OnNick;
			IRCConnection.Listener.OnNickError		+= Listener_OnNickError;
			IRCConnection.Connect ();

			IRCConnection.Sender.PrivateMessage ("nickserv", "identify " + ConfigurationManager.AppSettings["ircPassword"]);
			IRCConnection.Sender.Join (ConfigurationManager.AppSettings["ircChannel"]);

			IRCConnection.Listener.OnNames += Listener_OnNames;


			Messanger = new IRCMessanger (IRCConnection.Sender, ConfigurationManager.AppSettings["ircChannel"]);

			//new FileIOPermission (PermissionState.Unrestricted).Deny ();
			foreach (var plugin in Plugins)
			{
				plugin.Start (Messanger);
				Console.ForegroundColor = ConsoleColor.Blue;
				Console.WriteLine (plugin.GetType ().FullName + " module loaded.");
			}
		}

		static void Listener_OnNickError (string badNick, string reason)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine ("Nickname error: " + reason);
		}

		static void Listener_OnError (ReplyCode code, string message)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine (code + ": " + message);
		}

		static void Listener_OnDisconnected ()
		{
			Environment.Exit (1);
		}

		static void CurrentDomain_UnhandledException (object sender, UnhandledExceptionEventArgs e)
		{
			var ex = ((Exception)e.ExceptionObject);
			Console.WriteLine (ex.Message);
			Console.WriteLine (ex.StackTrace);

			Console.Error.WriteLine (ex.Message);
			Console.Error.WriteLine (ex.StackTrace);
			
			Messanger.SendToChannel ("Death is upon me: " + ex.Message);
			Environment.Exit (1);
		}

		static void Listener_OnJoin (UserInfo user, string channel)
		{
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.WriteLine ("[{0}] {1} has joined", channel, user.Nick);

			IRCConnection.Sender.Names (channel);
		}

		static void LoadPlugins (Assembly asm)
		{
			if (asm == null)
				return;

			object[] blank = new object[0];

			foreach (var p in asm.GetTypes().Where (t => t.GetInterface ("IPlugin") != null).Select (t => t.GetConstructor (Type.EmptyTypes)))
			{
				if (p != null)
					Plugins.Add ((IPlugin)p.Invoke (blank));
			}
		}

		static void Listener_OnNames (string channel, string[] nicks, bool last)
		{
			for (int i = 0; i < nicks.Length; ++i)
			{
				if (nicks[i].StartsWith ("@"))
					Operators.Add (nicks[i].Substring (1));
			}
		}

		static void Listener_OnPrivate (UserInfo user, string message)
		{
			Console.ForegroundColor = ConsoleColor.DarkBlue;
			Console.WriteLine ("[{0}]: {1}", user.Nick, message);

			foreach (var plugin in Plugins)
			{
				if ((plugin.Type & PluginType.PrivateParser) == PluginType.PrivateParser && plugin.IsRunning)
					plugin.ProcessPrivateMessage (user.Nick, message);
			}
		}

		static void Listener_OnPublic (UserInfo user, string channel, string message)
		{
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine ("[{0}] {1}: {2}", channel, user.Nick, message);

			foreach (var plugin in Plugins)
			{
				if ((plugin.Type & PluginType.Parser) == PluginType.Parser && plugin.IsRunning)
					plugin.ProcessPublicMessage (user.Nick, message);
			}
		}
	}
}