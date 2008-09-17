using System.IO;
using System.Xml;

using NAnt.Core;
using NAnt.Core.Attributes;

namespace NAntExtensions.TeamCity.Tasks
{
	public abstract class TeamCityBuildLogTaskBase : Task
	{
		string _teamCityInfoPath;

		[TaskAttribute("teamCityInfoPath")]
		public string TeamCityInfoPath
		{
			protected get
			{
				if (string.IsNullOrEmpty(_teamCityInfoPath))
				{
					_teamCityInfoPath = GetDefaultTeamCityInfoPath();
				}
				return _teamCityInfoPath;
			}
			set { _teamCityInfoPath = value; }
		}

		protected static XmlElement CreateStatisticValueNode(XmlDocument doc, string key, string value)
		{
			XmlElement element = doc.CreateElement("statisticValue");
			element.SetAttribute("key", key);
			element.SetAttribute("value", value);
			return element;
		}

		protected static XmlElement GetBuildNode(XmlDocument doc)
		{
			XmlElement documentElement = doc.DocumentElement;
			if (documentElement == null)
			{
				documentElement = doc.CreateElement("build");
				doc.AppendChild(documentElement);
			}
			return documentElement;
		}

		protected string GetDefaultTeamCityInfoPath()
		{
			string checkoutDir = Properties["teamcity.build.checkoutDir"];

			if (string.IsNullOrEmpty(checkoutDir))
			{
				return "teamcity-info.xml";
				//throw new BuildException(
				//    "Could not find the teamcity.build.checkoutDir property which must be specified if you don't explicitly set the 'teamCityInfoPath' attribute - is this a TeamCity build?");
			}
			return Path.Combine(checkoutDir, "teamcity-info.xml");
		}

		protected XmlDocument GetTeamCityLogXml()
		{
			XmlDocument document = new XmlDocument();
			if (File.Exists(TeamCityInfoPath))
			{
				document.Load(TeamCityInfoPath);
			}
			return document;
		}

		protected void SaveTeamCityLogXml(XmlDocument teamCityInfoXml)
		{
			teamCityInfoXml.Save(TeamCityInfoPath);
		}
	}
}