using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.BuildEnvironment;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_the_console_runner_debug_build_environment_is_inspected : Spec
	{
		DebugConsoleRunnerBuildEnvironment _sut;

		protected override void Before_each_spec()
		{
			_sut = new DebugConsoleRunnerBuildEnvironment();
		}

		[Test]
		public void Should_report_a_TeamCity_build()
		{
			Assert.IsTrue(_sut.IsTeamCityBuild);
		}

		[Test]
		public void Should_report_a_console_runner_build()
		{
			Assert.IsFalse(_sut.IsRunningWithTeamCityNAntRunner(null));
		}
	}
}