using System;

using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Types;

namespace NAntExtensions.TeamCity.Tasks
{
	[TaskName("tc-progress")]
	public class TeamCityProgress : TeamCityTask
	{
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

			Log(Level.Verbose, "Reporting progress. Type={0}, Message={1}", ProgressType, Message);
			
			switch (ProgressType)
			{
				case ProgressType.Message:
					Messaging.Message("##teamcity[progressMessage '{0}']", Message);
					break;
				case ProgressType.Start:
					Messaging.Message("##teamcity[progressStart '{0}']", Message);
					break;
				case ProgressType.End:
					Messaging.Message("##teamcity[progressFinish '{0}']", Message);
					break;
				default:
					throw new BuildException(String.Format("Unknown progress type: '{0}'", ProgressType));
			}
		}
	}
}