using System;

namespace SLAGG
{
	public static class Unix
	{
		public static DateTime Epoch
		{
			get { return Unix.epoch; }
		}

		public static ulong Now
		{
			get { return DateTime.Now.GetTimeT(); }
		}
		
		public static ulong Today
		{
			get { return DateTime.Today.GetTimeT(); }
		}

		public static ulong Tommorow
		{
			get { return DateTime.Today.AddDays (1).GetTimeT(); }
		}

		#region Extensions
		public static ulong GetTimeT (this DateTime time)
		{
			return (uint)Math.Round ((time.Subtract (Unix.epoch) - Unix.offset).TotalSeconds, 0);
		}

		public static DateTime GetDateTime (this long time_t)
		{
			return Unix.epoch.AddSeconds ((double)time_t + Unix.offset.TotalSeconds);
		}

		public static TimeSpan GetTimeSpan (this long time_t)
		{
			return (time_t.GetDateTime ().Subtract (Unix.GetDateTime (0)));
		}

		public static DateTime GetDateTime (this ulong time_t)
		{
			return Unix.epoch.AddSeconds ((double)time_t + Unix.offset.TotalSeconds);
		}

		public static TimeSpan GetTimeSpan (this ulong time_t)
		{
			return (time_t.GetDateTime ().Subtract (Unix.GetDateTime (0)));
		}

		public static DateTime GetDateTime (this uint time_t)
		{
			return Unix.epoch.AddSeconds ((double)time_t + Unix.offset.TotalSeconds);
		}

		public static TimeSpan GetTimeSpan (this double time_t)
		{
			return (((ulong)Math.Round (time_t, 0)).GetTimeSpan ());
		}

		public static DateTime GetDateTime (this double time_t)
		{
			return Unix.epoch.AddSeconds (time_t + Unix.offset.TotalSeconds);
		}
		#endregion

		private static TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset (DateTime.UtcNow);
		private static DateTime epoch = new DateTime (1970, 1, 1, 0, 0, 0);
	}
}