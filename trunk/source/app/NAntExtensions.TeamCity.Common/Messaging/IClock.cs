using System;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	/// <summary>
	/// A simple interface for a clock.
	/// </summary>
	public interface IClock
	{
		/// <summary>
		/// The current local time.
		/// </summary>
		DateTime Now
		{
			get;
		}

		/// <summary>
		/// The current UTC time.
		/// </summary>
		DateTime UtcNow
		{
			get;
		}
	}
}