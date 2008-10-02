using System;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	internal class SystemClock : IClock
	{
		#region IClock Members
		public DateTime Now
		{
			get { return DateTime.Now; }
		}

		public DateTime UtcNow
		{
			get { return DateTime.UtcNow; }
		}
		#endregion
	}
}