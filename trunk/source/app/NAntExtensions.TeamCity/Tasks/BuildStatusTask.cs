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
	/// <tc-buildstatus message="The build has failed"
	///                 type="Error" />
	/// ]]></code>
	/// </example>
	[TaskName("tc-buildstatus")]
	public class BuildStatusTask : MessageTask
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BuildStatusTask"/> class.
		/// </summary>
		public BuildStatusTask() : this(null, null)
		{
		}

		internal BuildStatusTask(IBuildEnvironment environment, ITeamCityMessageProvider messageProvider)
			: base(environment, messageProvider)
		{
		}

		/// <summary>
		/// The status type to report to TeamCity.
		/// </summary>
		/// <value>The type of the status.</value>
		[TaskAttribute("type", Required = false)]
		public StatusType StatusType
		{
			get;
			set;
		}

		/// <summary>
		/// The message to include with the report status.
		/// </summary>
		/// <value>The message.</value>
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

			Log(Level.Verbose, "Reporting build status. Type={0}, Message={1}", StatusType, Message ?? "(null)");

			MessageProvider.SendMessage("##teamcity[buildStatus status='{0}' text='{1}']",
			                            StatusType.ToString().ToUpperInvariant(),
			                            Message);
		}
	}
}