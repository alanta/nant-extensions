using System;

using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Messaging;
using NAntExtensions.TeamCity.Types;

namespace NAntExtensions.TeamCity.Tasks
{
	/// <summary>
	/// TODO
	/// </summary>
	/// <example>
	/// <code>
	/// <![CDATA[
	/// <tc-progress message="Running tests"
	///              type="Start" />
	/// ]]></code>
	/// </example>
	[TaskName("tc-progress")]
	public class ProgressTask : MessageTask
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProgressTask"/> class.
		/// </summary>
		public ProgressTask() : this(null, null)
		{
		}

		internal ProgressTask(IBuildEnvironment environment, ITeamCityMessageProvider messageProvider)
			: base(environment, messageProvider)
		{
		}

		/// <summary>
		/// The progress type to report to TeamCity.
		/// </summary>
		/// <value>The type of the progress.</value>
		[TaskAttribute("type", Required = false)]
		public ProgressType ProgressType
		{
			get;
			set;
		}
		
		/// <summary>
		/// The message to show on the TeamCity dashboard.
		/// </summary>
		[TaskAttribute("message", Required = false)]
		public string Message
		{
			get;
			set;
		}

		/// <summary>
		/// Executes the task.
		/// </summary>
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