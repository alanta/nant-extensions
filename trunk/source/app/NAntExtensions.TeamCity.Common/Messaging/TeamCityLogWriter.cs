using System;
using System.Globalization;
using System.IO;

using NAnt.Core;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	public abstract class TeamCityLogWriter : TextWriter
	{
		Task _task;

		protected TeamCityLogWriter() : base(CultureInfo.InvariantCulture)
		{
		}

		public Task Task
		{
			get { return _task; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_task = value;
			}
		}
	}
}