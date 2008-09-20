using System;

using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common.BuildEnvironment;

namespace NAntExtensions.TeamCity.Tasks
{
	public abstract class TeamCityTask : Task
	{
		protected IBuildEnvironment _buildEnvironment;

		protected TeamCityTask(IBuildEnvironment buildEnvironment)
		{
			BuildEnvironment = buildEnvironment;
		}

		[TaskAttribute("force")]
		[BooleanValidator]
		public bool ForceTaskExecution
		{
			get;
			set;
		}

		public IBuildEnvironment BuildEnvironment
		{
			get { return _buildEnvironment; }
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_buildEnvironment = value;
			}
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
	}
}