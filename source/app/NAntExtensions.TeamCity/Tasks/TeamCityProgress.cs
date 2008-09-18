using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Types;

namespace NAntExtensions.TeamCity.Tasks
{
	[TaskName("tc-progress")]
	public class TeamCityProgress : TeamCityBuildLogTaskBase
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
			if (!ShouldSkipTaskExecution)
			{
				return;
			}

			Log(Level.Verbose, "Reporting progress. Type={0}, Message={1}", ProgressType, Message);
			// TODO
			//switch (ProgressType)
			//{
			//    case ProgressType.Message:
			//        TeamCityReporter.RenderToLog(this, "##teamcity[progressMessage '{0}']", Message);
			//        break;
			//    case ProgressType.Start:
			//        TeamCityReporter.RenderToLog(this, "##teamcity[progressStart '{0}']", Message);
			//        break;
			//    case ProgressType.End:
			//        TeamCityReporter.RenderToLog(this, "##teamcity[progressFinish '{0}']", Message);
			//        break;
			//}
		}
	}
}