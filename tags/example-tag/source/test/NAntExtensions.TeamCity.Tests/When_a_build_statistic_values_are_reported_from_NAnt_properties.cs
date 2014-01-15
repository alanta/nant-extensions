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
	public class When_a_build_statistic_values_are_reported_from_NAnt_properties : Spec
	{
		const string Prefix = "test.";
		IBuildEnvironment _buildEnvironment;
		ITeamCityMessageProvider _messageProvider;
		PropertyDictionary _properties;
		AddStatisticFromPropertiesTask _task;

		protected override void Before_each_spec()
		{
			_messageProvider = Mocks.StrictMock<ITeamCityMessageProvider>();
			_buildEnvironment = Mocks.StrictMock<IBuildEnvironment>();

			_task = Mocks.PartialMock<AddStatisticFromPropertiesTask>(_buildEnvironment, _messageProvider);
			_task.ForceTaskExecution = true;

			// Logging is allowed at any time.
			_task.Log(Level.Debug, null);
			LastCall.IgnoreArguments().Repeat.Any();

			_properties = new PropertyDictionary(null)
			              { { "foo", "bar" }, { "test.foo", "test.bar" }, { "TEST.BAR", "TEST.BAZ" } };
		}

		[Test]
		public void Should_report_statistic_value_with_case_sensitive_matching()
		{
			_task.PropertiesStartingWith = Prefix;
			_task.IgnoreCase = false;

			using (Mocks.Record())
			{
				SetupResult.For(_task.Properties).Return(_properties);

				_messageProvider.BuildStatisticValue("test.foo", "test.bar");
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}

		[Test]
		public void Should_report_statistic_values_with_case_insensitive_matching()
		{
			_task.PropertiesStartingWith = Prefix;
			_task.IgnoreCase = true;

			using (Mocks.Record())
			{
				SetupResult.For(_task.Properties).Return(_properties);

				_messageProvider.BuildStatisticValue("test.foo", "test.bar");
				_messageProvider.BuildStatisticValue("TEST.BAR", "TEST.BAZ");
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}

		[Test]
		public void Should_not_report_statistic_values_when_no_properties_match()
		{
			_task.PropertiesStartingWith = "will not match";
			_task.IgnoreCase = true;

			using (Mocks.Record())
			{
				SetupResult.For(_task.Properties).Return(_properties);
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}
	}
}