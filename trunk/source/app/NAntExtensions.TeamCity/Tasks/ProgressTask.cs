using System;

using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Messaging;
using NAntExtensions.TeamCity.Types;

namespace NAntExtensions.TeamCity.Tasks
{
	[TaskName("tc-progress")]
	public class ProgressTask : MessageTask
	{
		public ProgressTask() : this(null, null)
		{
		}

		public ProgressTask(IBuildEnvironment environment, ITeamCityMessageProvider messageProvider)
			: base(environment, messageProvider)
		{
		}

		[TaskAttribute("type", Required = false)]
		public ProgressType ProgressType
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

			Log(Level.Verbose, "Reporting progress. Type={0}, Message={1}", ProgressType, Message ?? "(null)");

			switch (ProgressType)
			{
				case ProgressType.Message:
					MessageProvider.SendMessage("##teamcity[progressMessage '{0}']", Message);
					break;
				case ProgressType.Start:
					MessageProvider.SendMessage("##teamcity[progressStart '{0}']", Message);
					break;
				case ProgressType.End:
					MessageProvider.SendMessage("##teamcity[progressFinish '{0}']", Message);
					break;
				default:
					throw new BuildException(String.Format("Unknown progress type: '{0}'", ProgressType));
			}
		}
	}
}