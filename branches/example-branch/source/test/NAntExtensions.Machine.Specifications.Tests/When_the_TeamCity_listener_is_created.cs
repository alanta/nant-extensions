using System;

using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_the_TeamCity_listener_is_created : Spec
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Throws_exception_when_initialized_with_null_messaging()
		{
			new TeamCityRunListener(null);
		}
	}
}