using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Container;
using NAntExtensions.TeamCity.Common.Messaging;
using NAntExtensions.TeamCity.Types;

namespace NAntExtensions.TeamCity.Tasks
{
	[TaskName("tc-buildstatus")]
	public class BuildStatusTask : MessageTask
	{
		public BuildStatusTask()
			: this(IoC.Resolve<IBuildEnvironment>(), IoC.Resolve<ITeamCityMessageProvider>())
		{
		}

		public BuildStatusTask(IBuildEnvironment environment, ITeamCityMessageProvider messageProvider)
			: base(environment, messageProvider)
		{
		}

		[TaskAttribute("type", Required = false)]
		public StatusType StatusType
		{
			get;
			set;
		}

		[TaskAttribute("message", Required = false)]
		public string Message
		{
			get;
			set;
		}

		protected override void ExecuteTask()
		{
			if (ShouldSkipTaskExecution)
			{
				return;
			}

			Log(Level.Verbose, "Reporting build status. Type={0}, Message={1}", StatusType, Message??"(null)");

			MessageProvider.SendMessage("##teamcity[buildStatus status='{0}' text='{1}']",
			                        StatusType.ToString().ToUpperInvariant(),
			                        Message);
		}
	}
}