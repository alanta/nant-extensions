using Machine.Specifications.Runner;

using MbUnit.Framework;
using MbUnit.Framework.Reflection;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;

using Rhino.Mocks;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_the_NAnt_listener_monitors_context_ends : Spec
	{
		ContextInfo _contextInfo;
		NAntRunListener _sut;
		Task _task;

		protected override void Before_each_spec()
		{
      _contextInfo = new ContextInfo("Context", "Concern", "TypeName", "Namespace", "AssemblyName");

			PropertyDictionary propertyDictionary = Mocks.Stub<PropertyDictionary>((Project) null);

			_task = Mocks.DynamicMock<Task>();
			SetupResult.For(_task.Properties).Return(propertyDictionary);

			Mocks.Replay(_task);

			_sut = new NAntRunListener(_task);
		}

		[Test]
		public void Should_increment_the_context_count()
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_sut.OnContextEnd(_contextInfo);

				Assert.AreEqual(1, (int) Reflector.GetField(_sut, "_contextCount"));
			}
		}
	}
}