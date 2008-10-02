using System;

using MbUnit.Framework;
using MbUnit.Framework.Reflection;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.Messaging;

using Rhino.Mocks;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_a_message_is_logged_to_TeamCity : Spec
	{
		const string Message = "foo";
		const string MessageWithParameters = "foo{0}{1}{2}";
		ITeamCityMessageProvider _messageProvider;
		TeamCityLogWriter _writer;

		protected override void Before_each_spec()
		{
			_writer = Mocks.StrictMock<TeamCityLogWriter>();
			_messageProvider = new TeamCityMessageProvider(_writer, Mocks.StrictMock<Task>(), Mocks.StrictMock<IClock>());

			Mocks.BackToRecord(_writer);
		}

		[Test]
		public void Should_create_from_IClock_timestamp_with_correct_format()
		{
			TeamCityLogWriter writer = Mocks.StrictMock<TeamCityLogWriter>();
			IClock clock = Mocks.StrictMock<IClock>();
			TeamCityMessageProvider provider = new TeamCityMessageProvider(writer, Mocks.StrictMock<Task>(), clock);

			Mocks.BackToRecord(writer);
			
			using (Mocks.Record())
			{
				Expect.Call(clock.Now).Return(new DateTime(2008, 10, 03, 0, 34, 42, 345));
			}

			using (Mocks.Playback())
			{
				string timestamp = (string) Reflector.InvokeMethod(provider, "CreateTimestamp");
				Assert.AreEqual("timestamp='2008-10-03T00:34:42.3450000'", timestamp);
			}
		}
		
		[RowTest]
		[Row(SpecialValue.Null)]
		[Row("")]
		public void Does_not_log_if_message_is_null_or_empty(string message)
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_messageProvider.SendMessage(message);
			}
		}

		[Test]
		public void Can_log_message_without_parameters()
		{
			using (Mocks.Record())
			{
				_writer.WriteLine(Message);
			}

			using (Mocks.Playback())
			{
				_messageProvider.SendMessage(Message);
			}
		}

		[Test]
		public void Can_log_message_with_parameters()
		{
			using (Mocks.Record())
			{
				_writer.WriteLine(String.Format(MessageWithParameters, 1, 2, 3));
			}

			using (Mocks.Playback())
			{
				_messageProvider.SendMessage(MessageWithParameters, 1, 2, 3);
			}
		}

		[Test]
		public void Should_inject_timestamp_into_message()
		{
			using (Mocks.Record())
			{
				_writer.WriteLine(Message);
			}

			using (Mocks.Playback())
			{
				_messageProvider.SendMessage(Message);
			}
		}

	}
}