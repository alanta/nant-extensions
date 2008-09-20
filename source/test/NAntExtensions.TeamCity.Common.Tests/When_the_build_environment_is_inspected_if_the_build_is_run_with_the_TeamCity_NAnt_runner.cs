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
		TeamCityBuildEnvironment _sut;

		protected override void Before_each_spec()
		{
			_sut = new TeamCityBuildEnvironment(Mocks.StrictMock<IEnvironment>());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Should_throw_an_exception_if_the_passed_task_is_null()
		{
			_sut.IsRunningWithTeamCityNAntRunner(null);
		}

		[Test]
		public void Should_report_a_normal_build_if_task_properties_are_null()
		{
			Task task = Mocks.PartialMock<Task>();

			using (Mocks.Record())
			{
				SetupResult.For(task.Properties).Return(null);
			}

			using (Mocks.Playback())
			{
				Assert.IsFalse(_sut.IsRunningWithTeamCityNAntRunner(task));
			}
		}

		[Test]
		public void Should_report_a_normal_build_if_task_properties_do_not_contain_the_agent_name()
		{
			Task task = Mocks.PartialMock<Task>();

			using (Mocks.Record())
			{
				SetupResult.For(task.Properties).Return(new PropertyDictionary(null));
			}

			using (Mocks.Playback())
			{
				Assert.IsFalse(_sut.IsRunningWithTeamCityNAntRunner(task));
			}
		}

		[Test]
		public void Should_report_a_TeamCity_build_if_task_properties_do_contain_the_agent_name()
		{
			Task task = Mocks.PartialMock<Task>();

			using (Mocks.Record())
			{
				PropertyDictionary properties = new PropertyDictionary(null);
				properties.Add("agent.name", "42");
				SetupResult.For(task.Properties).Return(properties);
			}

			using (Mocks.Playback())
			{
				Assert.IsTrue(_sut.IsRunningWithTeamCityNAntRunner(task));
			}
		}
	}
}