using Machine.Specifications.Runner;

using MbUnit.Framework;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_the_NAnt_listener_monitors_assembly_starts : Spec
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
		public void Should_log_the_assembly_name()
		{
			Mocks.BackToRecord(_task);

			using (Mocks.Record())
			{
				_task.Log(Level.Info, null); 
				LastCall.Constraints(Is.Equal(Level.Info), Text.Contains(_assemblyInfo.Name));
			}

			using (Mocks.Playback())
			{
				_sut.OnAssemblyStart(_assemblyInfo);
			}
		}
	}
}