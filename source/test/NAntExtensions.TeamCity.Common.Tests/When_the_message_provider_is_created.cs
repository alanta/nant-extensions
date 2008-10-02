using System;

using MbUnit.Framework;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_the_message_provider_is_created : Spec
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Requires_an_instance_of_TeamCityLogWriter()
		{
			new TeamCityMessageProvider(null, Mocks.StrictMock<Task>(), Mocks.StrictMock<IClock>());
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Requires_an_instance_of_Task()
		{
			new TeamCityMessageProvider(Mocks.StrictMock<TeamCityLogWriter>(), null, Mocks.StrictMock<IClock>());
		}
		
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Requires_an_instance_of_IClock()
		{
			new TeamCityMessageProvider(Mocks.StrictMock<TeamCityLogWriter>(), Mocks.StrictMock<Task>(), null);
		}

		[Test]
		public void Assigns_the_task_to_the_TeamCityLogWriter()
		{
			TeamCityLogWriter teamCityLogWriter = Mocks.StrictMock<TeamCityLogWriter>();
			Task task = Mocks.StrictMock<Task>();
			IClock clock = Mocks.DynamicMock<IClock>();

			using (Mocks.Record())
			{
				teamCityLogWriter.TaskToUseForLogging = task;
			}

			using (Mocks.Playback())
			{
				new TeamCityMessageProvider(teamCityLogWriter, task, clock);
			}
		}
	}
}