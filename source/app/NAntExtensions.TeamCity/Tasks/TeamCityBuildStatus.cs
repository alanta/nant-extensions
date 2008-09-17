using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Tasks;
using NAntExtensions.TeamCity.Types;

namespace NAntExtensions.TeamCity.Tasks
{
	[TaskName("tc-buildstatus")]
	public class TeamCityBuildStatus : TeamCityBuildLogTaskBase
	{
		[TaskAttribute("type", Required = false)]
		public StatusType StatusType
		{
			private get;
			set;
		}

		[TaskAttribute("message", Required = false)]
		public string Message
		{
			private get;
			set;
		}

		protected override void ExecuteTask()
		{
			Log(Level.Verbose, "Reporting build status. Type={0}, Message={1}", StatusType, Message);
			// TODO
			//TeamCityReporter.RenderToLog(this,
			//                             "##teamcity[buildStatus status='{0}' text='{1}']",
			//                             StatusType.ToString().ToUpper(),
			//                             Message);
		}
	}
}