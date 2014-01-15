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
	public class When_tests_that_write_to_the_console_are_run : Spec
	{
		const string ConsoleErrorMessage = "Console.Error: foo bar baz";
		const string ConsoleOutMessage = "Console.Out: foo bar baz";
		TeamCityRunListener _listener;
		ITeamCityMessageProvider _messageProvider;
		SpecificationInfo _specificationInfo;

		protected override void Before_each_spec()
		{
			_specificationInfo = new SpecificationInfo("Spec", "Type");

			_messageProvider = Mocks.DynamicMock<ITeamCityMessageProvider>();
			_listener = new TeamCityRunListener(_messageProvider);

			_listener.OnContextStart(new ContextInfo("Context", "Concern", "TypeName", "Namespace", "AssemblyName"));
		}

		[Test]
		[MbUnit.Framework.Ignore("Functionality is disabled for now")]
		public void Should_report_the_Console_Out_stream_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestOutputStream(null, null);
				LastCall.Constraints(Is.Anything(), Is.Equal(ConsoleOutMessage));
			}

			_listener.OnSpecificationStart(_specificationInfo);

			using (Mocks.Playback())
			{
				Console.Write(ConsoleOutMessage);

				_listener.OnSpecificationEnd(_specificationInfo, Result.Pass());
			}
		}

		[Test]
		public void Should_not_report_the_Console_Out_stream_to_TeamCity_if_the_stream_is_empty()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestOutputStream(null, null);
				LastCall.IgnoreArguments().Repeat.Never();
			}

			_listener.OnSpecificationStart(_specificationInfo);

			using (Mocks.Playback())
			{
				_listener.OnSpecificationEnd(_specificationInfo, Result.Pass());
			}
		}

		[Test]
		[MbUnit.Framework.Ignore("Functionality is disabled for now")]
		public void Should_report_the_Console_Error_stream_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestErrorStream(null, null);
				LastCall.Constraints(Is.Anything(), Is.Equal(ConsoleErrorMessage));
			}

			_listener.OnSpecificationStart(_specificationInfo);

			using (Mocks.Playback())
			{
				Console.Error.Write(ConsoleErrorMessage);

				_listener.OnSpecificationEnd(_specificationInfo, Result.Pass());
			}
		}

		[Test]
		public void Should_not_report_the_Console_Error_stream_to_TeamCity_if_the_stream_is_empty()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestErrorStream(null, null);
				LastCall.IgnoreArguments().Repeat.Never();
			}

			_listener.OnSpecificationStart(_specificationInfo);

			using (Mocks.Playback())
			{
				_listener.OnSpecificationEnd(_specificationInfo, Result.Pass());
			}
		}
	}
}