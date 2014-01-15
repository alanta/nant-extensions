using System;

using Machine.Specifications;
using Machine.Specifications.Runner;

using MbUnit.Framework;
using MbUnit.Framework.Reflection;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_the_NAnt_listener_monitors_specification_ends : Spec
	{
		ContextInfo _contextInfo;
		SpecificationInfo _specificationInfo;
		NAntRunListener _sut;
		Task _task;

		protected override void Before_each_spec()
		{
			_contextInfo = new ContextInfo("Context", "Concern", "TypeName", "Namespace", "AssemblyName");
			_specificationInfo = new SpecificationInfo("Spec", "Type");

			PropertyDictionary propertyDictionary = Mocks.Stub<PropertyDictionary>((Project) null);

			_task = Mocks.DynamicMock<Task>();
			SetupResult.For(_task.Properties).Return(propertyDictionary);

			Mocks.Replay(_task);

			_sut = new NAntRunListener(_task);
		}

		[Test]
		public void Should_increment_the_success_count_if_the_spec_passed()
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_sut.OnSpecificationEnd(_specificationInfo, Result.Pass());
			}

			Assert.AreEqual(1, (int) Reflector.GetField(_sut, "_specificationCount"));
			Assert.AreEqual(1, (int) Reflector.GetField(_sut, "_passedSpecificationCount"));
		}

		[Test]
		public void Should_increment_the_failure_count_if_the_spec_failed()
		{
			using (Mocks.Record())
			{
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationEnd(_specificationInfo, Result.Failure(new Exception()));
			}

			Assert.AreEqual(1, (int) Reflector.GetField(_sut, "_specificationCount"));
			Assert.AreEqual(1, (int) Reflector.GetField(_sut, "_failedSpecificationCount"));
		}

		[Test]
		public void Should_set_error_status_if_the_spec_failed()
		{
			using (Mocks.Record())
			{
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationEnd(_specificationInfo, Result.Failure(new Exception()));
			}

			Assert.IsTrue(_sut.FailureOccurred);
		}

		[Test]
		public void Should_increment_the_ignore_count_if_the_spec_is_ignored()
		{
			using (Mocks.Record())
			{
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationEnd(_specificationInfo, Result.Ignored());
			}

			Assert.AreEqual(1, (int) Reflector.GetField(_sut, "_specificationCount"));
			Assert.AreEqual(1, (int) Reflector.GetField(_sut, "_ignoredSpecificationCount"));
		}

		[Test]
		public void Should_increment_the_unimplemented_count_if_the_spec_is_not_implemented()
		{
			using (Mocks.Record())
			{
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationEnd(_specificationInfo, Result.NotImplemented());
			}

			Assert.AreEqual(1, (int) Reflector.GetField(_sut, "_specificationCount"));
			Assert.AreEqual(1, (int) Reflector.GetField(_sut, "_unimplementedSpecificationCount"));
		}

		[Test]
		public void Should_increment_the_context_count_when_context_ends()
		{
			using (Mocks.Record())
			{
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnContextEnd(_contextInfo);
			}

			Assert.AreEqual(1, (int)Reflector.GetField(_sut, "_contextCount"));
		}

		[Test]
		public void Should_log_the_unimplemented_message_if_the_spec_is_not_implemented()
		{
			Mocks.BackToRecord(_task);

			using (Mocks.Record())
			{
				_task.Log(Level.Verbose, null);
				LastCall.Constraints(Is.Equal(Level.Verbose), Text.Contains("NOT IMPLEMENTED"));
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationEnd(_specificationInfo, Result.NotImplemented());
			}
		}

		[Test]
		public void Should_log_the_ignored_message_if_the_spec_is_ignored()
		{
			Mocks.BackToRecord(_task);

			using (Mocks.Record())
			{
				_task.Log(Level.Verbose, null);
				LastCall.Constraints(Is.Equal(Level.Verbose), Text.Contains("IGNORED"));
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationEnd(_specificationInfo, Result.Ignored());
			}
		}

		[Test]
		public void Should_log_the_error_message_without_spec_name_if_the_spec_failed_and_task_is_verbose()
		{
			Mocks.BackToRecord(_task);

			using (Mocks.Record())
			{
				SetupResult.For(_task.Verbose).Return(true);

				_task.Log(Level.Error, null);
				LastCall.Constraints(Is.Equal(Level.Error),
				                     Text.Contains("FAIL") && Text.Contains("Exception message") &&
				                     !Text.Contains(_contextInfo.Concern) && !Text.Contains(_specificationInfo.Name));
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationEnd(_specificationInfo, Result.Failure(new Exception("Exception message")));
			}
		}

		[Test]
		public void Should_log_the_error_message_and_spec_name_if_the_spec_failed_and_task_is_not_verbose()
		{
			Mocks.BackToRecord(_task);

			using (Mocks.Record())
			{
				SetupResult.For(_task.Verbose).Return(false);

				_task.Log(Level.Error, null);
				LastCall.Constraints(Is.Equal(Level.Error),
				                     Text.Contains("FAIL") && Text.Contains(_contextInfo.Concern) &&
				                     Text.Contains(_specificationInfo.Name));
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationEnd(_specificationInfo, Result.Failure(new Exception()));
			}
		}
	}
}
