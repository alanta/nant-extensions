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
	public class When_a_multiple_build_statistic_values_are_reported : Spec
	{
		const string InvalidPair = "foo";
		const string MultipleKeyValuePairs = "key1=value1;key2=value2";
		const string SingleKeyValuePair = "key=value";
		IBuildEnvironment _buildEnvironment;
		ITeamCityMessageProvider _messageProvider;
		AddStatisticListTask _task;

		protected override void Before_each_spec()
		{
			_messageProvider = Mocks.StrictMock<ITeamCityMessageProvider>();
			_buildEnvironment = Mocks.StrictMock<IBuildEnvironment>();

			_task = Mocks.PartialMock<AddStatisticListTask>(_buildEnvironment, _messageProvider);
			_task.ForceTaskExecution = true;

			// Logging is allowed at any time.
			_task.Log(Level.Debug, null);
			LastCall.IgnoreArguments().Repeat.Any();
		}

		[Test]
		public void Should_report_single_statistic_value()
		{
			_task.KeyValuePairs = SingleKeyValuePair;

			using (Mocks.Record())
			{
				_messageProvider.BuildStatisticValue("key", "value");
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}

		[Test]
		public void Should_report_multiple_statistic_values()
		{
			_task.KeyValuePairs = MultipleKeyValuePairs;

			using (Mocks.Record())
			{
				_messageProvider.BuildStatisticValue("key1", "value1");
				_messageProvider.BuildStatisticValue("key2", "value2");
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}

		[Test]
		public void Should_not_report_invalid_statistic_values()
		{
			_task.KeyValuePairs = InvalidPair;

			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}
	}
}