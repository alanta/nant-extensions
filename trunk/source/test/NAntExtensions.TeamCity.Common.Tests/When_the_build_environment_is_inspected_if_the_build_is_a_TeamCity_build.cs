using System;

using MbUnit.Framework;

using NAntExtensions.ForTesting;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_the_build_environment_is_inspected_if_the_build_is_a_TeamCity_build : Spec
	{
		const string TeamCityEnvironmentVariable = "TEAMCITY_PROJECT_NAME";

		protected override void After_each_spec()
		{
			Environment.SetEnvironmentVariable(TeamCityEnvironmentVariable, null, EnvironmentVariableTarget.Process);
		}

		[Test]
		[Explicit("Won't pass in a TeamCity build.")]
		public void Should_report_a_normal_build_if_TeamCity_environment_variable_does_not_exist()
		{
			Assert.IsFalse(BuildEnvironment.IsTeamCityBuild);
		}

		[Test]
		public void Should_report_a_TeamCity_build_if_TeamCity_environment_variable_does_exist()
		{
			Environment.SetEnvironmentVariable(TeamCityEnvironmentVariable, "42", EnvironmentVariableTarget.Process);
			Assert.IsTrue(BuildEnvironment.IsTeamCityBuild);
		}
	}
}