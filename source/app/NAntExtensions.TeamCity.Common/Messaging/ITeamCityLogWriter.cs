using System;

using NAnt.Core;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	public interface ITeamCityLogWriter:IDisposable
	{
		Task Task
		{
			set;
		}

		void WriteLine(string value);
	}
}