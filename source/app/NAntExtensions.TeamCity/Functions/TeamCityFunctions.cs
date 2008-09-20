using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Container;

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
			return IoC.Resolve<IBuildEnvironment>().IsTeamCityBuild;
		}
	}
}