using System;

using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.BuildEnvironment;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_the_build_environment_is_created : Spec
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Requires_an_instance_of_IBuildEnvironment()
		{
			new DefaultBuildEnvironment(null);
		}
	}
}