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

using NAntExtensions.TeamCity.Common;
using NAntExtensions.TeamCity.Common.Container;

namespace NAntExtensions.TeamCity.Tasks
{
	[TaskName("tc-addstatisticlist")]
	public class TeamCityAddStatisticList : TeamCityBuildLogTask
	{
		public TeamCityAddStatisticList() : this(IoC.Resolve<IBuildEnvironment>())
		{
		}

		public TeamCityAddStatisticList(IBuildEnvironment environment)
			: base(environment)
		{
		}

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
			if (ShouldSkipTaskExecution)
			{
				return;
			}

			Log(Level.Info, "Writing '{0}' to '{1}'", new object[] { KeyValuePairs, TeamCityInfoPath });

			XmlDocument teamCityInfo = LoadTeamCityInfo();
			XmlElement buildNode = GetBuildNode(teamCityInfo);

			AddKeyValuePairsToXml(teamCityInfo, buildNode);
			SaveTeamCityInfo(teamCityInfo);
		}
	}
}