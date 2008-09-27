using System;

using MbUnit.Framework;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.BuildEnvironment;

using Rhino.Mocks;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_the_build_environment_is_inspected_if_the_build_is_run_with_the_TeamCity_NAnt_runner : Spec
	{
		const string TeamCityEnvironmentVariable = "teamcity-dotnet-log-file";
		DefaultBuildEnvironment _sut;
		IEnvironment _environment;

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
				Assert.IsFalse(_sut.IsRunningWithTeamCityNAntRunner);
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
				Assert.IsTrue(_sut.IsRunningWithTeamCityNAntRunner);
			}
		}
	}
}