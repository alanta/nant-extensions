using MbUnit.Framework;
using MbUnit.Framework.Reflection;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;

using Rhino.Mocks;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_the_NAnt_listener_monitors_run_starts : Spec
	{
		NAntRunListener _sut;
		Task _task;

		protected override void Before_each_spec()
		{
			PropertyDictionary propertyDictionary = Mocks.Stub<PropertyDictionary>((Project) null);

			_task = Mocks.DynamicMock<Task>();
			SetupResult.For(_task.Properties).Return(propertyDictionary);

			Mocks.Replay(_task);

			_sut = new NAntRunListener(_task);
		}

		[Test]
		public void Should_log_an_informal_message()
		{
			Mocks.BackToRecord(_task);

			using (Mocks.Record())
			{
				_task.Log(Level.Info, "Running specs");
			}

			using (Mocks.Playback())
			{
				_sut.OnRunStart();
			}
		}

		[Test]
		public void Should_initialize_the_counters_with_zero()
		{
			using (Mocks.Record())
			{
			}

			Reflector.SetField(_sut, "_contextCount", int.MinValue);
			Reflector.SetField(_sut, "_failedSpecificationCount", int.MinValue);
			Reflector.SetField(_sut, "_ignoredSpecificationCount", int.MinValue);
			Reflector.SetField(_sut, "_passedSpecificationCount", int.MinValue);
			Reflector.SetField(_sut, "_specificationCount", int.MinValue);
			Reflector.SetField(_sut, "_unimplementedSpecificationCount", int.MinValue);

			using (Mocks.Playback())
			{
				_sut.OnRunStart();
			}

			Assert.AreEqual(0, Reflector.GetField(_sut, "_contextCount"));
			Assert.AreEqual(0, Reflector.GetField(_sut, "_failedSpecificationCount"));
			Assert.AreEqual(0, Reflector.GetField(_sut, "_ignoredSpecificationCount"));
			Assert.AreEqual(0, Reflector.GetField(_sut, "_passedSpecificationCount"));
			Assert.AreEqual(0, Reflector.GetField(_sut, "_specificationCount"));
			Assert.AreEqual(0, Reflector.GetField(_sut, "_unimplementedSpecificationCount"));
		}
	}
}