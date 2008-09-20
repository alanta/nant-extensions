using NAnt.Core;

namespace NAntExtensions.TeamCity.Common
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