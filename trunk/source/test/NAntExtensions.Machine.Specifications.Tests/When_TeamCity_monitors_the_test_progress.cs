using System;

using Machine.Specifications;
using Machine.Specifications.Runner;

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
		AssemblyInfo _assemblyInfo;
		ContextInfo _contextInfo;
		TeamCityRunListener _listener;
		ITeamCityMessageProvider _messageProvider;
		SpecificationInfo _specificationInfo;

		protected override void Before_each_spec()
		{
			_messageProvider = Mocks.StrictMock<ITeamCityMessageProvider>();
			_listener = new TeamCityRunListener(_messageProvider);

			_assemblyInfo = new AssemblyInfo("Assembly");
			_contextInfo = new ContextInfo("Context", "Concern");
			_specificationInfo = new SpecificationInfo("Spec");
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
				_messageProvider.TestSuiteStarted(null);
				LastCall.IgnoreArguments();
			}

			using (Mocks.Playback())
			{
				_listener.OnAssemblyStart(_assemblyInfo);
			}
		}

		[Test]
		public void Reports_assembly_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestSuiteFinished(null);
				LastCall.IgnoreArguments();
			}

			using (Mocks.Playback())
			{
				_listener.OnAssemblyEnd(_assemblyInfo);
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
				_messageProvider.TestStarted(null);
				LastCall.IgnoreArguments();
			}

			_listener.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_listener.OnSpecificationStart(_specificationInfo);
			}
		}

		[Test]
		public void Reports_successful_specification_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestStarted(null);
				LastCall.IgnoreArguments();

				_messageProvider.TestFinished(null);
				LastCall.IgnoreArguments();
			}

			_listener.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_listener.OnSpecificationStart(_specificationInfo);
				_listener.OnSpecificationEnd(_specificationInfo, Result.Pass());
			}
		}

		[Test]
		public void Reports_failed_specification_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestStarted(null);
				LastCall.IgnoreArguments();

				_messageProvider.TestFailed(null, null);
				LastCall.IgnoreArguments();

				_messageProvider.TestFinished(null);
				LastCall.IgnoreArguments();
			}

			_listener.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_listener.OnSpecificationStart(_specificationInfo);
				_listener.OnSpecificationEnd(_specificationInfo, Result.Failure(new Exception()));
			}
		}

		[Test]
		public void Reports_failed_specification_end_to_TeamCity_with_exception()
		{
			Exception exception = new InvalidOperationException();

			using (Mocks.Record())
			{
				_messageProvider.TestStarted(null);
				LastCall.IgnoreArguments();

				_messageProvider.TestFailed(null, null);
				LastCall.Constraints(Is.Anything(), Is.Equal(exception));

				_messageProvider.TestFinished(null);
				LastCall.IgnoreArguments();
			}

			_listener.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_listener.OnSpecificationStart(_specificationInfo);
				_listener.OnSpecificationEnd(_specificationInfo, Result.Failure(exception));
			}
		}
	}
}