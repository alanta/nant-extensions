using System;

using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.BuildEnvironment;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_the_system_environment_is_inspected : Spec
	{
		SystemEnvironment _sut;

		protected override void Before_each_spec()
		{
			_sut = new SystemEnvironment();
		}

		[Test]
		public void Should_report_a_TeamCity_build()
		{
			var environmentVariable = "COMPUTERNAME";
			Assert.AreEqual(Environment.GetEnvironmentVariable(environmentVariable), _sut.GetEnvironmentVariable(environmentVariable));
		}
	}
}