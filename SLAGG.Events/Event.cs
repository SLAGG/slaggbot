using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLAGG.Events
{
	public class Event
	{
		public Event (int eventId, string location, DateTime date)
		{
			this.eventId = eventId;
			this.Location = location;
			this.Date = date;
		}

		public string Location
		{
			get;
			private set;
		}

		public DateTime Date
		{
			get;
			private set;
		}

		public IEnumerable<string> Registered
		{
			get; set;
		}

		public override int GetHashCode ()
		{
			return eventId.GetHashCode();
		}

		private readonly int eventId;
	}
}