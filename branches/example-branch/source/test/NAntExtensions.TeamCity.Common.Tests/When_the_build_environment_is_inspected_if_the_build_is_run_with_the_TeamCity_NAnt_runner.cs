using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.BuildEnvironment;

using Rhino.Mocks;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_the_build_environment_is_inspected_if_the_build_is_run_with_the_TeamCity_NAnt_runner : Spec
	{
		const string TeamCityEnvironmentVariable1 = "teamcity-dotnet-log-file";
		const string TeamCityEnvironmentVariable2 = "BUILD_NUMBER_FORMAT";
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
				Expect.Call(_environment.GetEnvironmentVariable(TeamCityEnvironmentVariable1)).Return(null);
				Expect.Call(_environment.GetEnvironmentVariable(TeamCityEnvironmentVariable2)).Return(null);
			}

			using (Mocks.Playback())
			{
				Assert.IsFalse(_sut.IsRunningWithTeamCityNAntRunner);
			}
		}

		[Test]
		public void Should_report_a_TeamCity_build_if_TeamCity_environment_variable_does_exist()
		{
			using (Mocks.Record())
			{
				Expect.Call(_environment.GetEnvironmentVariable(TeamCityEnvironmentVariable1)).Return("foo");
			}

			using (Mocks.Playback())
			{
				Assert.IsTrue(_sut.IsRunningWithTeamCityNAntRunner);
			}
		}
	}
}