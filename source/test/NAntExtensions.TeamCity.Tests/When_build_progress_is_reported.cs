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
	public class When_build_progress_is_reported : Spec
	{
		const string Message = "foo";
		IBuildEnvironment _buildEnvironment;
		ITeamCityMessageProvider _messageProvider;
		ProgressTask _task;

		protected override void Before_each_spec()
		{
			_messageProvider = Mocks.StrictMock<ITeamCityMessageProvider>();
			_buildEnvironment = Mocks.StrictMock<IBuildEnvironment>();

			_task = Mocks.PartialMock<ProgressTask>(_buildEnvironment, _messageProvider);
			_task.ForceTaskExecution = true;

			// Logging is allowed at any time.
			_task.Log(Level.Debug, null);
			LastCall.IgnoreArguments().Repeat.Any();
		}

		[Test]
		[ExpectedException(typeof(BuildException))]
		public void Throws_exception_if_progress_type_is_unknown()
		{
			Reflector.SetProperty(_task, "ProgressType", int.MinValue);
			
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}

		[Test]
		public void Reports_start_message()
		{
			_task.ProgressType = ProgressType.Start;
			_task.Message = Message;

			using (Mocks.Record())
			{
				_messageProvider.SendMessage("##teamcity[progressStart '{0}']", Message);
				LastCall.Repeat.Once();
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}

		[Test]
		public void Reports_end_message()
		{
			_task.ProgressType = ProgressType.End;
			_task.Message = Message;

			using (Mocks.Record())
			{
				_messageProvider.SendMessage("##teamcity[progressFinish '{0}']", Message);
				LastCall.Repeat.Once();
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}

		[Test]
		public void Reports_message()
		{
			_task.ProgressType = ProgressType.Message;
			_task.Message = Message;

			using (Mocks.Record())
			{
				_messageProvider.SendMessage("##teamcity[progressMessage '{0}']", Message);
				LastCall.Repeat.Once();
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}
	}
}