using System;

using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Container;

namespace NAntExtensions.TeamCity.Tasks
{
	/// <summary>
	/// The base class for all tasks specific to TeamCity.
	/// </summary>
	public abstract class TeamCityTask : Task
	{
		IBuildEnvironment _buildEnvironment;

		/// <summary>
		/// Initializes a new instance of the <see cref="TeamCityTask"/> class.
		/// </summary>
		/// <param name="buildEnvironment">The build environment.</param>
		protected TeamCityTask(IBuildEnvironment buildEnvironment)
		{
			BuildEnvironment = buildEnvironment ?? IoC.Resolve<IBuildEnvironment>();
		}

		/// <summary>
		/// If <c>true</c> then the task is forced to execute regardless of running in a TeamCity build. The default value is <c>false</c>.
		/// </summary>
		/// <value><c>true</c> if task execution is forced; otherwise, <c>false</c>.</value>
		[TaskAttribute("force")]
		[BooleanValidator]
		public bool ForceTaskExecution
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the build environment.
		/// </summary>
		/// <value>The build environment.</value>
		protected internal IBuildEnvironment BuildEnvironment
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

		/// <summary>
		/// Gets a value indicating whether to skip the task execution.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the task execution should be skipped; otherwise, <c>false</c>.
		/// </value>
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