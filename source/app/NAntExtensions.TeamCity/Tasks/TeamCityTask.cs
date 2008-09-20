using System;

using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.TeamCity.Tasks
{
	public abstract class TeamCityTask : Task
	{
		ITeamCityMessageProvider _messageProvider;

		protected TeamCityTask()
		{
		}

		protected TeamCityTask(ITeamCityMessageProvider messageProvider)
		{
			MessageProvider = messageProvider;
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

		protected ITeamCityMessageProvider MessageProvider
		{
			get
			{
				if (_messageProvider == null)
				{
					MessageProvider =
						new TeamCityMessageProvider(new TeamCityLogWriter(this, BuildEnvironment.IsRunningWithTeamCityNAntRunner(this)));
				}
				return _messageProvider;
			}
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_messageProvider = value;
			}
		}
	}
}