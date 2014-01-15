using System.Diagnostics;

using Castle.MicroKernel;

using MbUnit.Framework;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.Container;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_the_container_configuration_is_initialized : Spec
	{
		[Test]
		public void Verify_that_all_Windsor_mappings_are_correct()
		{
			// We need to add a concrete instance of Task for this test because Task cannot be injected by means of
			// Windsor, but we require a Task as a ctor argument for DefaultTeamCityLogWriter.
			IoC.Container.Kernel.AddComponentInstance("Task", typeof(Task), Mocks.PartialMock<Task>());

			foreach (IHandler handler in IoC.Container.Kernel.GetAssignableHandlers(typeof(object)))
			{
				Debug.WriteLine(handler.ComponentModel.Name);
				IoC.Resolve(handler.ComponentModel.Name);
			}
		}
	}
}