using MbUnit.Framework;
using MbUnit.Framework.Reflection;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;
using NAntExtensions.Machine.Specifications.Types;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_the_NAnt_listener_monitors_run_ends : Spec
	{
		NAntRunListener _sut;
		Task _task;

		protected override void Before_each_spec()
		{
			PropertyDictionary propertyDictionary = Mocks.Stub<PropertyDictionary>((Project) null);

			_task = Mocks.StrictMock<Task>();
			SetupResult.For(_task.Properties).Return(propertyDictionary);

			Mocks.Replay(_task);

			_sut = new NAntRunListener(_task);
		}

		[Test]
		public void Should_add_current_counters_to_existing_NAnt_counters()
		{
			Mocks.BackToRecord(_task);

			Reflector.SetField(_sut, "_contextCount", 1);
			Reflector.SetField(_sut, "_specificationCount", 2);
			Reflector.SetField(_sut, "_failedSpecificationCount", 3);
			Reflector.SetField(_sut, "_passedSpecificationCount", 4);
			Reflector.SetField(_sut, "_ignoredSpecificationCount", 5);
			Reflector.SetField(_sut, "_unimplementedSpecificationCount", 6);

			using (Mocks.Record())
			{
				_task.Log(Level.Info, null);
				LastCall.Constraints(Is.Equal(Level.Info), Is.Anything());

				PropertyDictionary propertyDictionary = Mocks.StrictMock<PropertyDictionary>((Project) null);
				SetupResult.For(_task.Properties).Return(propertyDictionary);

				foreach (string counter in Counter.All)
				{
					SetupResult.For(propertyDictionary[counter]).Return("42");
				}

				propertyDictionary["mspec.contexts"] = "43";
				propertyDictionary["mspec.specs"] = "44";
				propertyDictionary["mspec.failedspecs"] = "45";
				propertyDictionary["mspec.passedspecs"] = "46";
				propertyDictionary["mspec.ignoredspecs"] = "47";
				propertyDictionary["mspec.unimplementedspecs"] = "48";
				
			}

			using (Mocks.Playback())
			{
				_sut.OnRunEnd();
			}
		}

		[Test]
		public void Should_log_successful_run_summary()
		{
			Mocks.BackToRecord(_task);

			Reflector.SetField(_sut, "_contextCount", 1);
			Reflector.SetField(_sut, "_specificationCount", 2);

			using (Mocks.Record())
			{
				_task.Log(Level.Info, null);
				LastCall.Constraints(Is.Equal(Level.Info), Text.Contains("Contexts: 1") && Text.Contains("Specifications: 2"));

				PropertyDictionary propertyDictionary = Mocks.Stub<PropertyDictionary>((Project)null);
				SetupResult.For(_task.Properties).Return(propertyDictionary);
			}

			using (Mocks.Playback())
			{
				_sut.OnRunEnd();
			}
		}
		
		[Test]
		public void Should_log_run_summary_with_failed_specs()
		{
			Mocks.BackToRecord(_task);

			Reflector.SetField(_sut, "_contextCount", 1);
			Reflector.SetField(_sut, "_specificationCount", 2);
			Reflector.SetField(_sut, "_failedSpecificationCount", 3);

			using (Mocks.Record())
			{
				_task.Log(Level.Info, null);
				LastCall.Constraints(Is.Equal(Level.Info), Text.Contains("3 failed"));

				PropertyDictionary propertyDictionary = Mocks.Stub<PropertyDictionary>((Project)null);
				SetupResult.For(_task.Properties).Return(propertyDictionary);
			}

			using (Mocks.Playback())
			{
				_sut.OnRunEnd();
			}
		}
		
		[Test]
		public void Should_log_run_summary_with_ignored_specs()
		{
			Mocks.BackToRecord(_task);

			Reflector.SetField(_sut, "_contextCount", 1);
			Reflector.SetField(_sut, "_specificationCount", 2);
			Reflector.SetField(_sut, "_ignoredSpecificationCount", 3);

			using (Mocks.Record())
			{
				_task.Log(Level.Info, null);
				LastCall.Constraints(Is.Equal(Level.Info), Text.Contains("3 ignored"));

				PropertyDictionary propertyDictionary = Mocks.Stub<PropertyDictionary>((Project)null);
				SetupResult.For(_task.Properties).Return(propertyDictionary);
			}

			using (Mocks.Playback())
			{
				_sut.OnRunEnd();
			}
		}[Test]
		public void Should_log_run_summary_with_unimplemented_specs()
		{
			Mocks.BackToRecord(_task);

			Reflector.SetField(_sut, "_contextCount", 1);
			Reflector.SetField(_sut, "_specificationCount", 2);
			Reflector.SetField(_sut, "_unimplementedSpecificationCount", 3);

			using (Mocks.Record())
			{
				_task.Log(Level.Info, null);
				LastCall.Constraints(Is.Equal(Level.Info), Text.Contains("3 not implemented"));

				PropertyDictionary propertyDictionary = Mocks.Stub<PropertyDictionary>((Project)null);
				SetupResult.For(_task.Properties).Return(propertyDictionary);
			}

			using (Mocks.Playback())
			{
				_sut.OnRunEnd();
			}
		}
	}
}