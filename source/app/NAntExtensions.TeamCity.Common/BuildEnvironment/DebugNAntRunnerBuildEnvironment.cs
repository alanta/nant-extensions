using NAnt.Core;

namespace NAntExtensions.TeamCity.Common.BuildEnvironment
{
	internal class DebugNAntRunnerBuildEnvironment : IBuildEnvironment
	{
		#region IBuildEnvironment Members
		public bool IsTeamCityBuild
		{
			get { return true; }
		}

		public bool IsRunningWithTeamCityNAntRunner(Task task)
		{
			return true;
		}
		#endregion
	}
}