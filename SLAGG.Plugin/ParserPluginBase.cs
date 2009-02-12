using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.Plugin
{
	public abstract class ParserPluginBase
		: IPlugin
	{
		#region IPlugin Members
		public virtual bool IsPublic
		{
			get { return true; }
		}

		public virtual PluginType Type
		{
			get { return PluginType.Parser | PluginType.PrivateParser; }
		}

		public bool IsRunning
		{
			get;
			protected set;
		}

		public virtual void Start (IMessanger messanger)
		{
			this.IsRunning = true;
			this.messanger = messanger;
		}

		public virtual void Stop ()
		{
			this.IsRunning = false;
			this.messanger = null;
		}

		public virtual void ProcessPublicMessage (string nick, string message)
		{
			if (this.IsRunning)
				this.messanger.SendToChannel (this.ProcessMessage (nick, message));
		}

		public virtual void ProcessPrivateMessage (string nick, string message)
		{
			if (this.IsRunning)
				this.messanger.SendToUser (nick, this.ProcessMessage (nick, message));
		}

		#endregion

		/// <summary>
		/// Processes any type of message and returns the response
		/// </summary>
		/// <param name="nick">The user that sent the message</param>
		/// <param name="message">The message</param>
		/// <returns>The response to the message (regardless of whether its public or private.)</returns>
		protected abstract string ProcessMessage (string nick, string message);

		protected IMessanger messanger;
	}
}
