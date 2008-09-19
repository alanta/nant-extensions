using System;
using System.Reflection;

using Machine.Specifications.Model;

using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;
using NAntExtensions.TeamCity.Common.Messaging;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace NAntExtensions.Machine.Specifications.Tests
{
	[TestFixture]
	public class When_TeamCity_monitors_the_test_progress : Spec
	{
		FieldInfo _fieldInfo;
		TeamCityRunListener _listener;
		Specification _specification;
		ITeamCityMessaging _teamCityMessaging;

		protected override void Before_each_spec()
		{
			_fieldInfo = Mocks.PartialMock<FieldInfo>();
			SetupResult.For(_fieldInfo.Name).Return("foo");
			Mocks.Replay(_fieldInfo);

			_specification = Mocks.PartialMock<Specification>(_fieldInfo, _fieldInfo);

			_teamCityMessaging = Mocks.StrictMock<ITeamCityMessaging>();
			_listener = new TeamCityRunListener(_teamCityMessaging);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Throws_exception_when_initialized_with_null_messaging()
		{
			new TeamCityRunListener(null);
		}

		[Test]
		public void Reports_assembly_start_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_teamCityMessaging.TestSuiteStarted(null);
				LastCall.IgnoreArguments();
			}

			using (Mocks.Playback())
			{
				_listener.OnAssemblyStart(GetType().Assembly);
			}
		}

		[Test]
		public void Reports_assembly_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_teamCityMessaging.TestSuiteFinished(null);
				LastCall.IgnoreArguments();
			}

			using (Mocks.Playback())
			{
				_listener.OnAssemblyEnd(GetType().Assembly);
			}
		}

		[Test]
		public void Does_not_report_run_start_to_TeamCity()
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_listener.OnRunStart();
			}
		}

		[Test]
		public void Does_not_report_run_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_listener.OnRunEnd();
			}
		}

		[Test]
		public void Does_not_report_context_start_to_TeamCity()
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_listener.OnContextStart(null);
			}
		}

		[Test]
		public void Does_not_report_context_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_listener.OnContextEnd(null);
			}
		}

		[Test]
		public void Reports_specification_start_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_teamCityMessaging.TestStarted(null);
				LastCall.IgnoreArguments();
			}

			_listener.OnContextStart(new Context(GetType(), null, null, null, null, null, null));

			using (Mocks.Playback())
			{
				_listener.OnSpecificationStart(_specification);
			}
		}

		[Test]
		public void Reports_successful_specification_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_teamCityMessaging.TestStarted(null);
				LastCall.IgnoreArguments();

				_teamCityMessaging.TestFinished(null);
				LastCall.IgnoreArguments();
			}

			_listener.OnContextStart(new Context(GetType(), null, null, null, null, null, null));

			using (Mocks.Playback())
			{
				_listener.OnSpecificationStart(_specification);
				_listener.OnSpecificationEnd(_specification, new SpecificationVerificationResult());
			}
		}

		[Test]
		public void Reports_failed_specification_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_teamCityMessaging.TestStarted(null);
				LastCall.IgnoreArguments();

				_teamCityMessaging.TestFailed(null, null);
				LastCall.IgnoreArguments();

				_teamCityMessaging.TestFinished(null);
				LastCall.IgnoreArguments();
			}

			_listener.OnContextStart(new Context(GetType(), null, null, null, null, null, null));

			using (Mocks.Playback())
			{
				_listener.OnSpecificationStart(_specification);
				_listener.OnSpecificationEnd(_specification, new SpecificationVerificationResult(Result.Failed));
			}
		}

		[Test]
		public void Reports_failed_specification_end_to_TeamCity_with_exception()
		{
			Exception exception = new InvalidOperationException();

			using (Mocks.Record())
			{
				_teamCityMessaging.TestStarted(null);
				LastCall.IgnoreArguments();

				_teamCityMessaging.TestFailed(null, null);
				LastCall.Constraints(Is.Anything(), Is.Equal(exception));

				_teamCityMessaging.TestFinished(null);
				LastCall.IgnoreArguments();
			}

			_listener.OnContextStart(new Context(GetType(), null, null, null, null, null, null));

			using (Mocks.Playback())
			{
				_listener.OnSpecificationStart(_specification);
				_listener.OnSpecificationEnd(_specification, new SpecificationVerificationResult(exception));
			}
		}
	}
}