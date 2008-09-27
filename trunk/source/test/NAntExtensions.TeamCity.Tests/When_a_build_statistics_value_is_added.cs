using System;
using System.IO;

using MbUnit.Framework;
using MbUnit.Framework.Reflection;

using NAnt.Core;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Tasks;

using Rhino.Mocks;

namespace NAntExtensions.TeamCity.Tests
{
	public class When_a_build_statistics_value_is_added : Spec
	{
		const string Key = "key";
		const string SecondKey = "key2";
		const string SecondValue = "bar";
		const string Value = "foo";
		IBuildEnvironment _buildEnvironment;
		AddStatisticTask _task;

		protected override void Before_each_spec()
		{
			_buildEnvironment = Mocks.StrictMock<IBuildEnvironment>();

			_task = Mocks.PartialMock<AddStatisticTask>(_buildEnvironment);
			_task.ForceTaskExecution = true;

			// Logging is allowed at any time.
			_task.Log(Level.Debug, null);
			LastCall.IgnoreArguments().Repeat.Any();

			using (Mocks.Record())
			{
				SetupResult.For(_task.Properties).Return(null);
			}

			if (File.Exists(_task.TeamCityInfoPath))
			{
				File.Delete(_task.TeamCityInfoPath);
			}
		}

		[Test]
		public void Creates_TeamCity_info_document()
		{
			_task.Key = Key;
			_task.Value = Value;

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}

			FileAssert.Exists(_task.TeamCityInfoPath);
		}

		[Test]
		public void Adds_first_statistic_value_to_document()
		{
			_task.Key = Key;
			_task.Value = Value;

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}

			string xml = File.ReadAllText(_task.TeamCityInfoPath);
			XmlAssert.XPathEvaluatesTo(String.Format("/build/statisticValue[@key='{0}']/@value", Key), xml, Value);
		}

		[Test]
		public void Adds_subsequent_statistic_values_to_document()
		{
			using (Mocks.Playback())
			{
				_task.Key = Key;
				_task.Value = Value;
				Reflector.InvokeMethod(_task, "ExecuteTask");

				_task.Key = SecondKey;
				_task.Value = SecondValue;
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}

			string xml = File.ReadAllText(_task.TeamCityInfoPath);

			XmlAssert.XPathEvaluatesTo(String.Format("/build/statisticValue[@key='{0}']/@value", Key), xml, Value);
			XmlAssert.XPathEvaluatesTo(String.Format("/build/statisticValue[@key='{0}']/@value", SecondKey), xml, SecondValue);
		}

		[Test]
		public void Overwrites_existing_values_for_a_given_key()
		{
			using (Mocks.Playback())
			{
				_task.Key = Key;
				_task.Value = Value;
				Reflector.InvokeMethod(_task, "ExecuteTask");

				_task.Value = SecondValue;
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}

			string xml = File.ReadAllText(_task.TeamCityInfoPath);

			XmlAssert.XPathEvaluatesTo(String.Format("/build/statisticValue[@key='{0}']/@value", Key), xml, SecondValue);
		}
	}
}