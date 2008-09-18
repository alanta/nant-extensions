using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Types;

namespace NAntExtensions.TeamCity.Tasks
{
	[TaskName("tc-buildstatus")]
	public class TeamCityBuildStatus : TeamCityBuildLogTaskBase
	{
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

			Log(Level.Verbose, "Reporting build status. Type={0}, Message={1}", StatusType, Message);
			// TODO
			//TeamCityReporter.RenderToLog(this,
			//                             "##teamcity[buildStatus status='{0}' text='{1}']",
			//                             StatusType.ToString().ToUpper(),
			//                             Message);
		}
	}
}