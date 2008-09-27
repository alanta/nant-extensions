using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.BuildEnvironment;

using Rhino.Mocks;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_the_build_environment_is_inspected_if_the_build_is_a_TeamCity_build : Spec
	{
		const string TeamCityEnvironmentVariable = "TEAMCITY_PROJECT_NAME";
		IEnvironment _environment;
		DefaultBuildEnvironment _sut;

		protected override void Before_each_spec()
		{
			_environment = Mocks.StrictMock<IEnvironment>();

			_sut = new DefaultBuildEnvironment(_environment);
		}

		[Test]
		public void Should_report_a_normal_build_if_TeamCity_environment_variable_does_not_exist()
		{
			using (Mocks.Record())
			{
				Expect.Call(_environment.GetEnvironmentVariable(TeamCityEnvironmentVariable)).Return(null);
			}

			using (Mocks.Playback())
			{
				Assert.IsFalse(_sut.IsTeamCityBuild);
			}
		}

		[Test]
		public void Should_report_a_TeamCity_build_if_TeamCity_environment_variable_does_exist()
		{
			using (Mocks.Record())
			{
				Expect.Call(_environment.GetEnvironmentVariable(TeamCityEnvironmentVariable)).Return("foo");
			}

			using (Mocks.Playback())
			{
				Assert.IsTrue(_sut.IsTeamCityBuild);
			}
		}
	}
}