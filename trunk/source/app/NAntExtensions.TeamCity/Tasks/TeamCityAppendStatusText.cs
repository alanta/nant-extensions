#region License
// Copyright (c) 2008, Bluewire Technologies Ltd.
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
//     * Neither the name of the Bluewire Technologies Ltd. nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS 
// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
#endregion

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