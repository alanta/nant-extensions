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
	public class When_the_TeamCity_listener_monitors_specification_ends : Spec
	{
		ContextInfo _contextInfo;
		ITeamCityMessageProvider _messageProvider;
		SpecificationInfo _specificationInfo;
		TeamCityRunListener _sut;

		protected override void Before_each_spec()
		{
			_messageProvider = Mocks.StrictMock<ITeamCityMessageProvider>();
			_sut = new TeamCityRunListener(_messageProvider);

			_contextInfo = new ContextInfo("Context", "Concern");
			_specificationInfo = new SpecificationInfo("Spec");
		}

		[Test]
		public void Should_report_successful_specification_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestStarted(null);
				LastCall.IgnoreArguments();

				_messageProvider.TestFinished(null);
				LastCall.IgnoreArguments();
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationStart(_specificationInfo);
				_sut.OnSpecificationEnd(_specificationInfo, Result.Pass());
			}
		}

		[Test]
		public void Should_report_failed_specification_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestStarted(null);
				LastCall.IgnoreArguments();

				_messageProvider.TestFailed(null, null, null, null);
				LastCall.IgnoreArguments();

				_messageProvider.TestFinished(null);
				LastCall.IgnoreArguments();
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationStart(_specificationInfo);
				_sut.OnSpecificationEnd(_specificationInfo, Result.Failure(new Exception()));
			}
		}

		[Test]
		public void Should_report_failed_specification_end_to_TeamCity_with_exception()
		{
			Exception exception = new InvalidOperationException();

			using (Mocks.Record())
			{
				_messageProvider.TestStarted(null);
				LastCall.IgnoreArguments();

				_messageProvider.TestFailed(null, null, null, null);
				LastCall.Constraints(Is.Anything(), Is.Equal(exception.Message), Is.Null(), Is.Equal(exception.GetType().FullName));

				_messageProvider.TestFinished(null);
				LastCall.IgnoreArguments();
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationStart(_specificationInfo);
				_sut.OnSpecificationEnd(_specificationInfo, Result.Failure(exception));
			}
		}

		[Test]
		public void Should_report_ignored_specification_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestStarted(null);
				LastCall.IgnoreArguments();

				_messageProvider.TestIgnored(null, null);
				LastCall.IgnoreArguments();

				_messageProvider.TestFinished(null);
				LastCall.IgnoreArguments();
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationStart(_specificationInfo);
				_sut.OnSpecificationEnd(_specificationInfo, Result.Ignored());
			}
		}

		[Test]
		public void Should_report_unimplemented_specification_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestStarted(null);
				LastCall.IgnoreArguments();

				_messageProvider.TestIgnored(null, null);
				LastCall.IgnoreArguments();

				_messageProvider.TestFinished(null);
				LastCall.IgnoreArguments();
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationStart(_specificationInfo);
				_sut.OnSpecificationEnd(_specificationInfo, Result.NotImplemented());
			}
		}
	}
}