using Machine.Specifications.Runner;

using MbUnit.Framework;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_the_NAnt_listener_monitors_context_starts : Spec
	{
		ContextInfo _contextInfo;
		NAntRunListener _sut;
		Task _task;

		protected override void Before_each_spec()
		{
      _contextInfo = new ContextInfo("Context", "Concern", "TypeName", "Namespace", "AssemblyName");

			PropertyDictionary propertyDictionary = Mocks.Stub<PropertyDictionary>((Project) null);

			_task = Mocks.StrictMock<Task>();
			SetupResult.For(_task.Properties).Return(propertyDictionary);

			Mocks.Replay(_task);

			_sut = new NAntRunListener(_task);
		}

		[Test]
		public void Should_log_the_context_name()
		{
			Mocks.BackToRecord(_task);

			using (Mocks.Record())
			{
				_task.Log(Level.Verbose, null);
				LastCall.Constraints(Is.Equal(Level.Verbose), Text.Contains(_contextInfo.FullName));
			}

			using (Mocks.Playback())
			{
				_sut.OnContextStart(_contextInfo);
			}
		}
	}
}