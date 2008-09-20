using NAnt.Core;

namespace NAntExtensions.TeamCity.Common.BuildEnvironment
{
	public interface IBuildEnvironment
	{
		bool IsTeamCityBuild
		{
			get;
		}

		bool IsRunningWithTeamCityNAntRunner(Task task);
	}
}