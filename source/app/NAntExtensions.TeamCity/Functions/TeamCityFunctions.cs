using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common;

namespace NAntExtensions.TeamCity.Functions
{
	[FunctionSet("teamcity", "TeamCity")]
	public class TeamCityFunctions : FunctionSetBase
	{
		public TeamCityFunctions(Project project, PropertyDictionary properties) : base(project, properties)
		{
		}

		[Function("is-teamcity-build")]
		public static bool IsTeamCityBuild()
		{
			return BuildEnvironment.IsTeamCityBuild;
		}
	}
}