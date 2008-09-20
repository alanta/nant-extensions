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
	[TaskName("tc-addstatistic")]
	public class TeamCityAddStatistic : TeamCityBuildLogTask
	{
		public TeamCityAddStatistic() : this(IoC.Resolve<IBuildEnvironment>())
		{
		}

		public TeamCityAddStatistic(IBuildEnvironment environment) : base(environment)
		{
		}

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
			if (ShouldSkipTaskExecution)
			{
				return;
			}

			Log(Level.Info, "Writing '{0}={1}' to '{2}'", Key, Value, TeamCityInfoPath);

			XmlDocument teamCityInfo = LoadTeamCityInfo();
			XmlElement buildNode = GetBuildNode(teamCityInfo);
			XmlElement newChild = CreateStatisticValueNode(teamCityInfo, Key, Value);

			buildNode.AppendChild(newChild);
			SaveTeamCityInfo(teamCityInfo);
		}
	}
}