using System;

using MbUnit.Framework;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_the_message_privider_is_created : Spec
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Requires_an_instance_of_TeamCityLogWriter()
		{
			new TeamCityMessageProvider(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Requires_an_instance_of_Task()
		{
			TeamCityMessageProvider provider = new TeamCityMessageProvider(Mocks.StrictMock<TeamCityLogWriter>());
			provider.Task = null;
		}

		[Test]
		public void Assigns_the_task_to_the_TeamCityLogWriter()
		{
			TeamCityLogWriter teamCityLogWriter = Mocks.StrictMock<TeamCityLogWriter>();
			TeamCityMessageProvider provider = new TeamCityMessageProvider(teamCityLogWriter);
			Task task = Mocks.StrictMock<Task>();

			using (Mocks.Record())
			{
				teamCityLogWriter.Task = task;
			}

			using (Mocks.Playback())
			{
				provider.Task = task;
			}
		}
	}
}