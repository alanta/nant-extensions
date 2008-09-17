using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace NAntExtensions.TeamCity.Tasks
{
	[TaskName("tc-appendstatustext")]
	public class TeamCityAppendStatusText : TeamCityBuildLogTaskBase
	{
		[TaskAttribute("value")]
		public string Value
		{
			private get;
			set;
		}

		void AppendStatusText(XmlDocument doc, XmlElement statusInfoNode, string text)
		{
			XmlElement newChild = doc.CreateElement("text");
			newChild.SetAttribute("action", "append");
			newChild.InnerText = text;
			statusInfoNode.AppendChild(newChild);
		}

		protected override void ExecuteTask()
		{
			Log(Level.Info, "Writing '{0}' to '{1}'", new object[] { Value, TeamCityInfoPath });

			XmlDocument teamCityLogXml = GetTeamCityLogXml();
			XmlElement buildNode = GetBuildNode(teamCityLogXml);
			XmlElement statusInfoNode = GetStatusInfoNode(teamCityLogXml, buildNode);

			AppendStatusText(teamCityLogXml, statusInfoNode, Value);
			SaveTeamCityLogXml(teamCityLogXml);
		}

		XmlElement GetStatusInfoNode(XmlDocument doc, XmlElement buildNode)
		{
			XmlElement newChild = buildNode.SelectSingleNode("statusInfo") as XmlElement;
			if (newChild == null)
			{
				newChild = doc.CreateElement("statusInfo");
				buildNode.AppendChild(newChild);
			}
			return newChild;
		}
	}
}