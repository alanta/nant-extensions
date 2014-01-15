using MbUnit.Framework;
using MbUnit.Framework.Reflection;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Messaging;
using NAntExtensions.TeamCity.Tasks;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace NAntExtensions.TeamCity.Tests
{
	[TypeFixture(typeof(TeamCityTask))]
	[ProviderFactory(typeof(TaskFactory), typeof(TeamCityTask))]
	public class When_tasks_are_executed : TypeSpec
	{
		#region Factory
		public class TaskFactory
		{
			readonly IBuildEnvironment _buildEnvironment;
			readonly ITeamCityMessageProvider _messageProvider;

			public TaskFactory()
			{
				_messageProvider = InternalMocks.DynamicMock<ITeamCityMessageProvider>();
				_buildEnvironment = InternalMocks.StrictMock<IBuildEnvironment>();
			}

			[Factory]
			public TeamCityTask AddStatisticList
			{
				get
				{
					AddStatisticListTask task = InternalMocks.PartialMock<AddStatisticListTask>(_buildEnvironment, _messageProvider);
					SetupResult.For(task.Properties).Return(null);
					task.KeyValuePairs = "foo=bar";
					return task;
				}
			}

			[Factory]
			public TeamCityTask AddStatisticFromProperties
			{
				get
				{
					TeamCityTask task = InternalMocks.PartialMock<AddStatisticFromPropertiesTask>(_buildEnvironment, _messageProvider);
					SetupResult.For(task.Properties).Return(null);
					return task;
				}
			}

			[Factory]
			public TeamCityTask AddStatistic
			{
				get
				{
					TeamCityTask task = InternalMocks.PartialMock<AddStatisticTask>(_buildEnvironment, _messageProvider);
					SetupResult.For(task.Properties).Return(null);
					return task;
				}
			}

			[Factory]
			public TeamCityTask StatusText
			{
				get
				{
					TeamCityTask task = InternalMocks.PartialMock<StatusTextTask>(_buildEnvironment);
					SetupResult.For(task.Properties).Return(null);
					return task;
				}
			}

			[Factory]
			public TeamCityTask BuildStatus
			{
				get { return InternalMocks.PartialMock<BuildStatusTask>(_buildEnvironment, _messageProvider); }
			}

			[Factory]
			public TeamCityTask Progress
			{
				get { return InternalMocks.PartialMock<ProgressTask>(_buildEnvironment, _messageProvider); }
			}

			[Factory]
			public TeamCityTask BuildNumber
			{
				get { return InternalMocks.PartialMock<BuildNumberTask>(_buildEnvironment, _messageProvider); }
			}

			[Factory]
			public TeamCityTask PublishArtifacts
			{
				get { return InternalMocks.PartialMock<PublishArtifactsTask>(_buildEnvironment, _messageProvider); }
			}

			[Factory]
			public TeamCityTask ImportData
			{
				get { return InternalMocks.PartialMock<ImportDataTask>(_buildEnvironment, _messageProvider); }
			}
		}
		#endregion

		protected static MockRepository InternalMocks = new MockRepository();

		public When_tasks_are_executed() : base(InternalMocks)
		{
		}

		[Test]
		public void Skips_task_execution_when_run_outside_of_TeamCity(TeamCityTask task)
		{
			task.ForceTaskExecution = false;

			using (Mocks.Record())
			{
				SetupResult.For(task.BuildEnvironment.IsTeamCityBuild).Return(false);

				task.Log(Level.Verbose, null);
				LastCall.Constraints(Is.Equal(Level.Verbose), Text.Contains("Skipping task execution"));
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(task, "ExecuteTask");
			}
		}

		[Test]
		public void Executes_the_task_when_run_inside_a_TeamCity_build(TeamCityTask task)
		{
			task.ForceTaskExecution = false;

			using (Mocks.Record())
			{
				SetupResult.For(task.BuildEnvironment.IsTeamCityBuild).Return(true);

				task.Log(Level.Verbose, null);
				LastCall.Constraints(Is.Equal(Level.Verbose), Text.Contains("Skipping task execution")).Repeat.Never();

				task.Log(Level.Verbose, null);
				LastCall.Constraints(Is.Anything(), Is.Anything()).Repeat.Any();
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(task, "ExecuteTask");
			}
		}
	}
}