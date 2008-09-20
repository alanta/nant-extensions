using System.Diagnostics;

using Castle.MicroKernel;

using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.Container;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_the_container_configuration_is_initialized : Spec
	{
		[Test]
		public void Verify_that_all_Windsor_mappings_are_correct()
		{
			foreach (IHandler handler in IoC.Container.Kernel.GetAssignableHandlers(typeof(object)))
			{
				Debug.WriteLine(handler.ComponentModel.Name);

				IoC.Resolve(handler.ComponentModel.Name);
			}
		}
	}
}