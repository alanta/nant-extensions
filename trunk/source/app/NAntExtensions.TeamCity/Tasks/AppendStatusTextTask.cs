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

using NAntExtensions.TeamCity.Common.BuildEnvironment;

namespace NAntExtensions.TeamCity.Tasks
{
	/// <summary>
	/// Appends a status message to the TeamCity build status to <c>teamcity-info.xml</c>.
	/// </summary>
	/// <remarks>This task will only be executed within a TeamCity build.</remarks>
	/// <example>
	/// <code>
	/// <![CDATA[
	/// <tc-appendstatustext value=" Code Coverage ${coverage.value}" />
	/// ]]></code>
	/// </example>
	/// <seealso href="http://www.jetbrains.net/confluence/display/TCD3/Build+Script+Interaction+with+TeamCity#BuildScriptInteractionwithTeamCity-ModifyingtheBuildStatus">
	/// Build Script Interaction with TeamCity</seealso>
	[TaskName("tc-appendstatustext")]
	public class AppendStatusTextTask : BuildLogTask
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AppendStatusTextTask"/> class.
		/// </summary>
		public AppendStatusTextTask() : this(null)
		{
		}

		internal AppendStatusTextTask(IBuildEnvironment environment) : base(environment)
		{
		}

		/// <summary>
		/// The message to append to the build status node.
		/// </summary>
		/// <value>The value.</value>
		[TaskAttribute("message")]
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

			Log(Level.Info, "Writing '{0}' to '{1}'", Message ?? "(null)", TeamCityInfoPath);

			XmlDocument teamCityInfo = LoadTeamCityInfo();
			XmlElement buildNode = GetBuildNode(teamCityInfo);
			XmlElement statusInfoNode = GetStatusInfoNode(teamCityInfo, buildNode);

			AppendStatusText(teamCityInfo, statusInfoNode, Message);
			SaveTeamCityInfo(teamCityInfo);
		}

		static void AppendStatusText(XmlDocument doc, XmlElement statusInfoNode, string text)
		{
			XmlElement statusTextNode = doc.CreateElement("text");
			statusTextNode.SetAttribute("action", "append");
			statusTextNode.InnerText = text;

			statusInfoNode.AppendChild(statusTextNode);
		}

		static XmlElement GetStatusInfoNode(XmlDocument doc, XmlElement buildNode)
		{
			XmlElement statusInfoNode = buildNode.SelectSingleNode("statusInfo") as XmlElement;
			if (statusInfoNode == null)
			{
				statusInfoNode = doc.CreateElement("statusInfo");
				buildNode.AppendChild(statusInfoNode);
			}
			return statusInfoNode;
		}
	}
}