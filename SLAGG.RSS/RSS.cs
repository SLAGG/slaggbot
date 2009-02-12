using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLAGG.Plugin;
using System.Threading;
using Rss;
using System.Configuration;

namespace SLAGG.RSS
{
	public class RSS
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

			this.rssListener = new Thread (this.Listener)
			{
				Name = "RSS Listener",
				IsBackground = true
			};
			this.rssListener.Start ();
		}

		public void Stop ()
		{
			this.listening = false;
			
			if (this.rssListener != null)
				this.rssListener.Join ();

			this.messanger = null;
		}

		public void ProcessPublicMessage (string nick, string message)
		{
			return;
		}

		public void ProcessPrivateMessage (string nick, string message)
		{
			return;
		}

		#endregion

		private RssFeedSection config;
		protected RssFeedSection Config
		{
			get
			{
				if (this.config == null)
					this.config = (RssFeedSection)ConfigurationManager.GetSection ("feeds");

				return this.config;
			}
		}

		private Dictionary<RssFeedElement, KeyValuePair<DateTime, RssFeed>> oldFeeds = new Dictionary<RssFeedElement, KeyValuePair<DateTime, RssFeed>> ();

		private IMessanger messanger;
		private bool listening;
		private Thread rssListener;
		private void Listener ()
		{
			while (this.listening)
			{
				foreach (RssFeedElement feed in this.Config.Feeds)
				{
					DateTime lastChecked = DateTime.Now;

					RssFeed f = null;
					if (this.oldFeeds.ContainsKey (feed))
					{
						f = RssFeed.Read (this.oldFeeds[feed].Value);
						lastChecked = this.oldFeeds[feed].Key;
						this.oldFeeds[feed] = new KeyValuePair<DateTime, RssFeed> (DateTime.Now, f);
					}
					else
					{
						f = RssFeed.Read (feed.FeedURL);
						this.oldFeeds.Add (feed, new KeyValuePair<DateTime, RssFeed> (DateTime.Now, f));
					}					

					for (int c = 0; c < f.Channels.Count; ++c)
					{
						var channel = f.Channels[c];
						foreach (RssItem item in channel.Items.Cast<RssItem>().OrderBy (i => i.PubDate))
						{
							if (item.PubDate.ToUniversalTime() < lastChecked.ToUniversalTime())
								continue;

							messanger.SendToChannel (feed.Name + ": " + item.Title + " | " + item.Link);
						}
					}
				}

				Thread.Sleep (60000);
			}
		}
	}
}