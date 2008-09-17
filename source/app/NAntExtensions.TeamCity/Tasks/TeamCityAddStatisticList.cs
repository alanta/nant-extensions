using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace NAntExtensions.TeamCity.Tasks
{
	[TaskName("tc-addstatisticlist")]
	public class TeamCityAddStatisticList : TeamCityBuildLogTaskBase
	{
		[TaskAttribute("keyValuePairs")]
		public string KeyValuePairs
		{
			private get;
			set;
		}

		void AddKeyValuePairsToXml(XmlDocument teamCityInfoXml, XmlElement buildNode)
		{
			foreach (string str in KeyValuePairs.Split(new[] { ';' }))
			{
				string[] strArray = str.Split(new[] { '=' });
				if (strArray.Length > 1)
				{
					XmlElement newChild = CreateStatisticValueNode(teamCityInfoXml, strArray[0], strArray[1]);
					buildNode.AppendChild(newChild);
				}
			}
		}

		protected override void ExecuteTask()
		{
			Log(Level.Info, "Writing '{0}' to '{1}'", new object[] { KeyValuePairs, TeamCityInfoPath });

			XmlDocument teamCityLogXml = GetTeamCityLogXml();
			XmlElement buildNode = GetBuildNode(teamCityLogXml);

			AddKeyValuePairsToXml(teamCityLogXml, buildNode);
			SaveTeamCityLogXml(teamCityLogXml);
		}
	}
}