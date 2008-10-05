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
	public class When_build_artifacts_are_published : Spec
	{
		const string PathToArtifacts = "foo/*.zip";
		IBuildEnvironment _buildEnvironment;
		ITeamCityMessageProvider _messageProvider;
		PublishArtifactsTask _task;

		protected override void Before_each_spec()
		{
			_messageProvider = Mocks.StrictMock<ITeamCityMessageProvider>();
			_buildEnvironment = Mocks.StrictMock<IBuildEnvironment>();

			_task = Mocks.PartialMock<PublishArtifactsTask>(_buildEnvironment, _messageProvider);
			_task.ForceTaskExecution = true;

			// Logging is allowed at any time.
			_task.Log(Level.Debug, null);
			LastCall.IgnoreArguments().Repeat.Any();
		}

		[Test]
		public void Reports_build_number()
		{
			_task.PathToArtifacts = PathToArtifacts;

			using (Mocks.Record())
			{
				_messageProvider.PublishBuildArtifacts(PathToArtifacts);
				LastCall.Repeat.Once();
			}

			using (Mocks.Playback())
			{
				Reflector.InvokeMethod(_task, "ExecuteTask");
			}
		}
	}
}