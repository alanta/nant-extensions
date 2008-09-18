using System;

using NAnt.Core;

namespace NAntExtensions.TeamCity.Common
{
	public class BuildEnvironment
	{
		public static bool IsTeamCityBuild
		{
			get { return Environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME") != null; }
		}

		public static bool IsRunningWithTeamCityNAntRunner(Task task)
		{
			return task.Properties.Contains("agent.name");
		}
	}
}