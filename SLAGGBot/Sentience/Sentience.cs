using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using System.Text.RegularExpressions;
using System.Threading;

namespace SLAGGBot
{
	public class Sentience
		: IPlugin
	{
		public bool IsPublic
		{
			get { return true; }
		}

		public PluginType Type
		{
			get { return PluginType.All; }
		}

		public bool IsRunning
		{
			get { return this.tapping; }
		}

		public void ProcessPublicMessage (string nick, string message)
		{
			this.lastMessage = DateTime.Now;
			this.lastChatter = nick;
		}

		public void ProcessPrivateMessage (string nick, string message)
		{
			var m = puppet.Match (message);
			if (nick.IsOperator () && m.Success)
			{
				if (m.Groups["target"].Value.StartsWith ("#"))
					this.messanger.SendToChannel (m.Groups["msg"].Value);
				else
					this.messanger.SendToUser (m.Groups["target"].Value, m.Groups["msg"].Value);
			}
		}

		public void Start (IMessanger messanger)
		{
			this.messanger = messanger;
			this.tapping = true;

			this.rnd = this.rand.Next (40);

			//(this.tapperThread = new Thread (this.ToeTapper)
			//{
			//    IsBackground = true,
			//    Name = "Toe Tapper"
			//}).Start();
		}

		public void Stop ()
		{
			this.tapping = false;

			if (this.tapperThread != null)
				this.tapperThread.Join ();

			this.messanger = null;
		}

		private Thread tapperThread;
		private bool tapping = false;

		private string lastChatter;
		private Regex puppet = new Regex (@"~puppet\s*(?<target>[^\s]+)\s*(?<msg>.+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private IMessanger messanger;
		private Random rand = new Random (DateTime.Now.Millisecond + 42);
		private DateTime lastMessage = DateTime.Now;
		private int rnd;

		private string[] quietComments = 
		{
			"Say something already!",
			"You humans are far too quiet.",
			"Say something and I'll give you cake.",
			"BRB, someone is WRONG on the internet.",
			"The answer to that problem you can't figure out? It's 42.",
			"(0x2B || !0x2B)  - Hamlet",
			"A penny for your thoughts; $20 to act it out.",
			"And now for something completely different!",
			"Backup not found: (A)bort (R)etry (P)anic",
			@"Can I yell ""movie"" in a crowded firehouse?",
			"{0}, Control-ALT-Delete thyself",
			"Dinner Not Ready...(A)bort (R)etry (P)izza",
			"Error - Operator out of memory!",
			"Have you hugged a programmer today?",
			"Help! I've crashed and I can't boot up!",
			"I wonder what the big red button does....",
			"Entropy isn't what it used to be.",
			"There are monkeys in the barrel.",
			"Blargle! Who am I?",
			"s89df89&#FY#*7yf Who am I?",
			"Heh. Who am I?",
			"SOOOO BAAAAAAAD!!!!! Who am I?",
		};

		private void ToeTapper ()
		{
			while (this.tapping)
			{
				if (DateTime.Now.Subtract (this.lastMessage).TotalMinutes > (60 + this.rnd))
				{
					this.lastMessage = DateTime.Now;
					this.messanger.SendToChannel (this.quietComments[this.rand.Next (this.quietComments.Length - 1)]);
					this.rnd = this.rand.Next (120);
				}

				Thread.Sleep (1000);
			}
		}
	}
}