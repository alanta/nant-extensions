using System;

using MbUnit.Framework;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.Messaging;

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
			_messageProvider = new TeamCityMessageProvider(_writer, Mocks.StrictMock<Task>());

			Mocks.BackToRecord(_writer);
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
	}
}