using MbUnit.Framework;
using MbUnit.Framework.Reflection;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Messaging;
using NAntExtensions.TeamCity.Tasks;

using Rhino.Mocks;

namespace NAntExtensions.TeamCity.Tests
{
	public class When_a_single_build_statistic_value_is_reported : Spec
	{
		const string Key = "key";
		const string Value = "value";
		IBuildEnvironment _buildEnvironment;
		ITeamCityMessageProvider _messageProvider;
		AddStatisticTask _task;

		protected override void Before_each_spec()
		{
			_messageProvider = Mocks.StrictMock<ITeamCityMessageProvider>();
			_buildEnvironment = Mocks.StrictMock<IBuildEnvironment>();

			_task = Mocks.PartialMock<AddStatisticTask>(_buildEnvironment, _messageProvider);
			_task.ForceTaskExecution = true;

			// Logging is allowed at any time.
			_task.Log(Level.Debug, null);
			LastCall.IgnoreArguments().Repeat.Any();
		}

		[Test]
		public void Should_report_statistic_value()
		{
			_task.Key = Key;
			_task.Value = Value;

			using (Mocks.Record())
			{
				_messageProvider.BuildStatisticValue(_task.Key, _task.Value);
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}
	}
}