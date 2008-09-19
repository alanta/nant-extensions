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
	public class When_tests_that_write_to_the_console_are_run : Spec
	{
		const string ConsoleErrorMessage = "Console.Error: foo bar baz";
		const string ConsoleOutMessage = "Console.Out: foo bar baz";
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

			_teamCityMessaging = Mocks.DynamicMock<ITeamCityMessaging>();
			_listener = new TeamCityRunListener(_teamCityMessaging);
			_listener.OnContextStart(new Context(GetType(), null, null, null, null, null, null));
		}

		[Test]
		public void Should_report_the_Console_Out_stream_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_teamCityMessaging.TestOutputStream(null, null);
				LastCall.Constraints(Is.Anything(), Is.Equal(ConsoleOutMessage));
			}

			_listener.OnSpecificationStart(_specification);

			using (Mocks.Playback())
			{
				Console.Write(ConsoleOutMessage);

				_listener.OnSpecificationEnd(_specification, new SpecificationVerificationResult());
			}
		}

		[Test]
		public void Should_not_report_the_Console_Out_stream_to_TeamCity_if_the_stream_is_empty()
		{
			using (Mocks.Record())
			{
				_teamCityMessaging.TestOutputStream(null, null);
				LastCall.IgnoreArguments().Repeat.Never();
			}

			_listener.OnSpecificationStart(_specification);

			using (Mocks.Playback())
			{
				_listener.OnSpecificationEnd(_specification, new SpecificationVerificationResult());
			}
		}

		[Test]
		public void Should_report_the_Console_Error_stream_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_teamCityMessaging.TestErrorStream(null, null);
				LastCall.Constraints(Is.Anything(), Is.Equal(ConsoleErrorMessage));
			}

			_listener.OnSpecificationStart(_specification);

			using (Mocks.Playback())
			{
				Console.Error.Write(ConsoleErrorMessage);

				_listener.OnSpecificationEnd(_specification, new SpecificationVerificationResult());
			}
		}

		[Test]
		public void Should_not_report_the_Console_Error_stream_to_TeamCity_if_the_stream_is_empty()
		{
			using (Mocks.Record())
			{
				_teamCityMessaging.TestErrorStream(null, null);
				LastCall.IgnoreArguments().Repeat.Never();
			}

			_listener.OnSpecificationStart(_specification);

			using (Mocks.Playback())
			{
				_listener.OnSpecificationEnd(_specification, new SpecificationVerificationResult());
			}
		}
	}
}