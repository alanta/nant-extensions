using System;

using NAnt.Core;

namespace NAntExtensions.TeamCity.Common
{
	public class BuildEnvironment : IBuildEnvironment
	{
		readonly IEnvironment _environment;

		public BuildEnvironment(IEnvironment environment)
		{
			if (environment == null)
			{
				throw new ArgumentNullException("environment");
			}

			_environment = environment;
		}

		public bool IsTeamCityBuild
		{
			get { return _environment.GetEnvironmentVariable("TEAMCITY_PROJECT_NAME") != null; }
		}

		public bool IsRunningWithTeamCityNAntRunner(Task task)
		{
			if (task == null)
			{
				throw new ArgumentNullException("task");
			}

			return task.Properties != null && task.Properties.Contains("agent.name");
		}
	}
}