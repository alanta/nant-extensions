using System;

using MbUnit.Framework;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.Messaging;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_testing_related_messages_are_logged_to_TeamCity : Spec
	{
		const string AssemblyName = "Assembly";
		const string Stream = "Stream";
		const string TestName = "Test";
		IClock _clock;
		ITeamCityMessageProvider _messageProvider;
		TeamCityLogWriter _writer;

		protected override void Before_each_spec()
		{
			_writer = Mocks.StrictMock<TeamCityLogWriter>();
			_clock = Mocks.DynamicMock<IClock>();
			_messageProvider = new TeamCityMessageProvider(_writer, Mocks.StrictMock<Task>(), _clock);

			Mocks.BackToRecord(_writer);
		}

		[Test]
		public void Should_log_TestSuiteStarted()
		{
			using (Mocks.Record())
			{
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.StartsWith("##teamcity[testSuiteStarted") && Text.Contains(AssemblyName));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestSuiteStarted(AssemblyName);
			}
		}

		[Test]
		public void Should_log_TestSuiteStarted_with_timestamp()
		{
			using (Mocks.Record())
			{
				Expect.Call(_clock.Now).Return(DateTime.MaxValue);
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.Contains(" timestamp='"));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestSuiteStarted(AssemblyName);
			}
		}

		[RowTest]
		[Row(SpecialValue.Null)]
		[Row("")]
		public void Should_not_log_TestSuiteStarted_if_assembly_name_is_empty(string assemblyName)
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestSuiteStarted(assemblyName);
			}
		}

		[Test]
		public void Should_log_TestSuiteFinished()
		{
			using (Mocks.Record())
			{
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.StartsWith("##teamcity[testSuiteFinished") && Text.Contains(AssemblyName));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestSuiteFinished(AssemblyName);
			}
		}

		[Test]
		public void Should_log_TestSuiteFinished_with_timestamp()
		{
			using (Mocks.Record())
			{
				Expect.Call(_clock.Now).Return(DateTime.MaxValue);
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.Contains(" timestamp='"));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestSuiteFinished(AssemblyName);
			}
		}

		[RowTest]
		[Row(SpecialValue.Null)]
		[Row("")]
		public void Should_not_log_TestSuiteFinished_if_assembly_name_is_empty(string assemblyName)
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestSuiteFinished(assemblyName);
			}
		}

		[Test]
		public void Should_log_TestStarted()
		{
			using (Mocks.Record())
			{
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.StartsWith("##teamcity[testStarted") && Text.Contains(TestName));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestStarted(TestName);
			}
		}

		[Test]
		public void Should_log_TestStarted_with_timestamp()
		{
			using (Mocks.Record())
			{
				Expect.Call(_clock.Now).Return(DateTime.MaxValue);
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.Contains(" timestamp='"));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestStarted(TestName);
			}
		}

		[RowTest]
		[Row(SpecialValue.Null)]
		[Row("")]
		public void Should_not_log_TestStarted_if_test_name_is_empty(string testName)
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestStarted(testName);
			}
		}

		[Test]
		public void Should_log_TestFinished()
		{
			using (Mocks.Record())
			{
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.StartsWith("##teamcity[testFinished") && Text.Contains(TestName));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestFinished(TestName);
			}
		}

		[Test]
		public void Should_log_TestFinished_with_timestamp()
		{
			using (Mocks.Record())
			{
				Expect.Call(_clock.Now).Return(DateTime.MaxValue);
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.Contains(" timestamp='"));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestFinished(TestName);
			}
		}

		[RowTest]
		[Row(SpecialValue.Null)]
		[Row("")]
		public void Should_not_log_TestFinished_if_test_name_is_empty(string testName)
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestFinished(testName);
			}
		}

		[Test]
		public void Should_log_TestIgnored()
		{
			using (Mocks.Record())
			{
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.StartsWith("##teamcity[testIgnored") && Text.Contains(TestName));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestIgnored(TestName, null);
			}
		}

		[Test]
		public void Should_log_TestIgnored_with_timestamp()
		{
			using (Mocks.Record())
			{
				Expect.Call(_clock.Now).Return(DateTime.MaxValue);
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.Contains(" timestamp='"));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestIgnored(TestName, null);
			}
		}

		[RowTest]
		[Row(SpecialValue.Null)]
		[Row("")]
		public void Should_not_log_TestIgnored_if_test_name_is_empty(string testName)
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestIgnored(testName, null);
			}
		}

		[Test]
		public void Should_log_TestFailed_without_exception()
		{
			using (Mocks.Record())
			{
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.StartsWith("##teamcity[testFailed") && Text.Contains(TestName));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestFailed(TestName, (Exception) null);
			}
		}

		[Test]
		public void Should_log_TestFailed_without_exception_with_timestamp()
		{
			using (Mocks.Record())
			{
				Expect.Call(_clock.Now).Return(DateTime.MaxValue);
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.Contains(" timestamp='"));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestFailed(TestName, (Exception) null);
			}
		}

		[Test]
		public void Should_log_TestFailed_with_exception()
		{
			Exception exception = new Exception("Some error");

			using (Mocks.Record())
			{
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.Contains(String.Format("type='{0}'", exception.GetType().FullName)));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestFailed(TestName, exception);
			}
		}[Test]
		public void Should_log_TestFailed_with_exception_with_timestamp()
		{
			Exception exception = new Exception("Some error");

			using (Mocks.Record())
			{
				Expect.Call(_clock.Now).Return(DateTime.MaxValue);
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.Contains(" timestamp='"));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestFailed(TestName, exception);
			}
		}

		[RowTest]
		[Row(SpecialValue.Null)]
		[Row("")]
		public void Should_not_log_TestFailed_if_test_name_is_empty(string testName)
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestFailed(testName, (Exception) null);
			}
		}

		[Test]
		public void Should_log_TestStdOut()
		{
			using (Mocks.Record())
			{
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.StartsWith("##teamcity[testStdOut") && Text.Contains(TestName) && Text.Contains(Stream));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestOutputStream(TestName, Stream);
			}
		}

		[Test]
		public void Should_log_TestStdOut_with_timestamp()
		{
			using (Mocks.Record())
			{
				Expect.Call(_clock.Now).Return(DateTime.MaxValue);
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.Contains(" timestamp='"));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestOutputStream(TestName, Stream);
			}
		}

		[RowTest]
		[Row(SpecialValue.Null)]
		[Row("")]
		public void Should_not_log_TestStdOut_if_test_name_is_empty(string testName)
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestOutputStream(testName, null);
			}
		}

		[Test]
		public void Should_log_TestStdErr()
		{
			using (Mocks.Record())
			{
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.StartsWith("##teamcity[testStdErr") && Text.Contains(TestName) && Text.Contains(Stream));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestErrorStream(TestName, Stream);
			}
		}

		[Test]
		public void Should_log_TestStdErr_with_timestamp()
		{
			using (Mocks.Record())
			{
				Expect.Call(_clock.Now).Return(DateTime.MaxValue);
				_writer.WriteLine(String.Empty);
				LastCall.Constraints(Text.Contains(" timestamp='"));
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestErrorStream(TestName, Stream);
			}
		}

		[RowTest]
		[Row(SpecialValue.Null)]
		[Row("")]
		public void Should_not_log_TestStdErr_if_test_name_is_empty(string testName)
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_messageProvider.TestErrorStream(testName, null);
			}
		}
	}
}