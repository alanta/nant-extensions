using System;

using NAnt.Core;

namespace NAntExtensions.TeamCity.Common.BuildEnvironment
{
	internal class TeamCityBuildEnvironment : IBuildEnvironment
	{
		readonly IEnvironment _environment;

		public TeamCityBuildEnvironment(IEnvironment environment)
		{
			if (environment == null)
			{
				throw new ArgumentNullException("environment");
			}

			_environment = environment;
		}

		#region IBuildEnvironment Members
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
		#endregion
	}
}