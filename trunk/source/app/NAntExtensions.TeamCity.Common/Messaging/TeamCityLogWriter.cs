using System.Globalization;
using System.IO;

using NAnt.Core;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	public abstract class TeamCityLogWriter : TextWriter
	{
		protected TeamCityLogWriter() : base(CultureInfo.InvariantCulture)
		{
		}

		public abstract Task Task
		{
			get;
			set;
		}
	}
}