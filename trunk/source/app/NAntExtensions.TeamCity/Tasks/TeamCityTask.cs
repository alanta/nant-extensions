using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.TeamCity.Tasks
{
	public abstract class TeamCityTask : Task
	{
		protected TeamCityTask()
		{
			Messaging = new TeamCityMessaging(new TeamCityLogWriter(this, BuildEnvironment.IsRunningWithTeamCityNAntRunner(this)));
		}

		[TaskAttribute("force")]
		[BooleanValidator]
		public bool ForceTaskExecution
		{
			get;
			set;
		}

		protected bool ShouldSkipTaskExecution
		{
			get
			{
				if (ForceTaskExecution)
				{
					Log(Level.Verbose, "Not running as part of a TeamCity build. Forced task execution.");
					return false;
				}

				if (!BuildEnvironment.IsTeamCityBuild)
				{
					Log(Level.Verbose,
					    "Not running as part of a TeamCity build. Skipping task execution. Force task execution by setting the 'force' attribute to 'true'.");
				}

				return !BuildEnvironment.IsTeamCityBuild;
			}
		}

		protected TeamCityMessaging Messaging
		{
			get;
			private set;
		}
	}
}