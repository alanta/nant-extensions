using System;

namespace NAntExtensions.TeamCity.Common.BuildEnvironment
{
	internal class DefaultBuildEnvironment : IBuildEnvironment
	{
		readonly IEnvironment _environment;

		public DefaultBuildEnvironment(IEnvironment environment)
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

		public bool IsRunningWithTeamCityNAntRunner
		{
			get
			{
				// HACK: This relies on an implementation detail. The NAnt runner sets this environment
				// variable whereas the Command Line runner does not.
				return _environment.GetEnvironmentVariable("teamcity-dotnet-log-file") != null;
			}
		}
		#endregion
	}
}