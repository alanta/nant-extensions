using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace NAntExtensions.TeamCity.Tasks
{
	[TaskName("tc-addstatistic")]
	public class TeamCityAddStatistic : TeamCityBuildLogTaskBase
	{
		[TaskAttribute("key", Required = true)]
		public string Key
		{
			private get;
			set;
		}

		[TaskAttribute("value", Required = true)]
		public string Value
		{
			private get;
			set;
		}

		protected override void ExecuteTask()
		{
			Log(Level.Info, "Writing '{0}={1}' to '{2}'", new object[] { Key, Value, TeamCityInfoPath });

			XmlDocument teamCityLogXml = GetTeamCityLogXml();
			XmlElement buildNode = GetBuildNode(teamCityLogXml);
			XmlElement newChild = CreateStatisticValueNode(teamCityLogXml, Key, Value);

			buildNode.AppendChild(newChild);
			SaveTeamCityLogXml(teamCityLogXml);
		}
	}
}