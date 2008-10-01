using System;
using System.Collections;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common.BuildEnvironment;

namespace NAntExtensions.TeamCity.Tasks
{
	[TaskName("tc-addstatistic-fromprops")]
	public class AddStatisticFromPropertiesTask : BuildLogTask
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AddStatisticFromPropertiesTask"/> class.
		/// </summary>
		public AddStatisticFromPropertiesTask() : this(null)
		{
		}

		internal AddStatisticFromPropertiesTask(IBuildEnvironment environment) : base(environment)
		{
			IgnoreCase = true;
		}

		/// <summary>
		/// Adds TeamCity statistics values for all NAnt properties starting with this strings.
		/// </summary>
		/// <value>The properties starting with.</value>
		[TaskAttribute("starting-with", Required = true)]
		public string PropertiesStartingWith
		{
			get;
			set;
		}

		/// <summary>
		/// Compare property names case insensitive. The default value is 'true'.
		/// </summary>
		[TaskAttribute("ignore-case")]
		public bool IgnoreCase
		{
			get;
			set;
		}

		StringComparison PropertyKeyComparison
		{
			get
			{
				if (IgnoreCase)
				{
					return StringComparison.OrdinalIgnoreCase;
				}

				return StringComparison.Ordinal;
			}
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

			Log(Level.Info,
			    "Creating statistic values from properties starting with '{0}' (case {1}sensitive)",
			    PropertiesStartingWith,
			    IgnoreCase ? "in" : String.Empty);

			XmlDocument teamCityInfo = LoadTeamCityInfo();
			XmlElement buildNode = GetBuildNode(teamCityInfo);

			foreach (DictionaryEntry property in Properties)
			{
				if (!property.Key.ToString().StartsWith(PropertiesStartingWith, PropertyKeyComparison))
				{
					continue;
				}

				Log(Level.Info, "Writing '{0}={1}' to '{2}'", property.Key, property.Value, TeamCityInfoPath);

				XmlElement newChild = CreateStatisticValueNode(teamCityInfo, property.Key.ToString(), property.Value.ToString());
				buildNode.AppendChild(newChild);

				SaveTeamCityInfo(teamCityInfo);
			}
		}
	}
}