using MbUnit.Framework;
using MbUnit.Framework.Reflection;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Messaging;
using NAntExtensions.TeamCity.Tasks;
using NAntExtensions.TeamCity.Types;

using Rhino.Mocks;

namespace NAntExtensions.TeamCity.Tests
{
	public class When_build_status_is_reported : Spec
	{
		const string Message = "foo";
		IBuildEnvironment _buildEnvironment;
		ITeamCityMessageProvider _messageProvider;
		BuildStatusTask _task;

		protected override void Before_each_spec()
		{
			_messageProvider = Mocks.StrictMock<ITeamCityMessageProvider>();
			_buildEnvironment = Mocks.StrictMock<IBuildEnvironment>();

			_task = Mocks.PartialMock<BuildStatusTask>(_buildEnvironment, _messageProvider);
			_task.ForceTaskExecution = true;

			// Setting the task on the message provider through MessageTask.set_MessageProvider() is not an expectation.
			Mocks.BackToRecord(_messageProvider);

			// Logging is allowed at any time.
			_task.Log(Level.Debug, null);
			LastCall.IgnoreArguments().Repeat.Any();
		}

		[CombinatorialTest]
		public void Reports_status([UsingEnum(typeof(StatusType))] StatusType status)
		{
			_task.StatusType = status;
			_task.Message = Message;

			using (Mocks.Record())
			{
				_messageProvider.SendMessage("##teamcity[buildStatus status='{0}' text='{1}']",
				                             status.ToString().ToUpperInvariant(),
				                             Message);
				LastCall.Repeat.Once();
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}
	}
}