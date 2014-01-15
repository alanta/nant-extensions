using Machine.Specifications.Runner;

using MbUnit.Framework;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;

using Rhino.Mocks;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_the_NAnt_listener_monitors_assembly_ends : Spec
	{
		NAntRunListener _sut;
		Task _task;
		AssemblyInfo _assemblyInfo;

		protected override void Before_each_spec()
		{
			_assemblyInfo = new AssemblyInfo("Assembly");

			PropertyDictionary propertyDictionary = Mocks.Stub<PropertyDictionary>((Project) null);

			_task = Mocks.StrictMock<Task>();
			SetupResult.For(_task.Properties).Return(propertyDictionary);

			Mocks.Replay(_task);

			_sut = new NAntRunListener(_task);
		}

		[Test]
		public void Should_not_log_anything()
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_sut.OnAssemblyEnd(_assemblyInfo);
			}
		}
	}
}