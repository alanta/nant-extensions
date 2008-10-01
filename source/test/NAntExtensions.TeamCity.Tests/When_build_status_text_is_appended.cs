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
	public class When_build_status_text_is_appended : Spec
	{
		const string SecondValue = "bar";
		const string Value = "foo";
		IBuildEnvironment _buildEnvironment;
		AppendStatusTextTask _task;

		protected override void Before_each_spec()
		{
			_buildEnvironment = Mocks.StrictMock<IBuildEnvironment>();

			_task = Mocks.PartialMock<AppendStatusTextTask>(_buildEnvironment);
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
			_task.Message = Value;

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}

			FileAssert.Exists(_task.TeamCityInfoPath);
		}

		[Test]
		public void Appends_first_status_message_to_document()
		{
			_task.Message = Value;

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}

			string xml = File.ReadAllText(_task.TeamCityInfoPath);
			XmlAssert.XPathEvaluatesTo("/build/statusInfo/text[@action='append']", xml, Value);
		}

		[Test]
		public void Appends_subsequent_status_messages_to_document()
		{
			using (Mocks.Playback())
			{
				_task.Message = Value;
				Reflector.InvokeMethod(_task, "ExecuteTask");
				_task.Message = SecondValue;
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}

			string xml = File.ReadAllText(_task.TeamCityInfoPath);
			XmlAssert.XPathEvaluatesTo("/build/statusInfo/text[@action='append']", xml, Value + SecondValue);
		}
	}
}