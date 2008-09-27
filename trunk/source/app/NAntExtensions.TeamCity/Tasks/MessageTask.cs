using System;

using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Container;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.TeamCity.Tasks
{
	public abstract class MessageTask : TeamCityTask
	{
		ITeamCityMessageProvider _messageProvider;

		protected MessageTask(IBuildEnvironment buildEnvironment, ITeamCityMessageProvider messageProvider)
			: base(buildEnvironment)
		{
			MessageProvider = messageProvider ?? IoC.Resolve<ITeamCityMessageProvider>(new object[] { this });
		}

		protected ITeamCityMessageProvider MessageProvider
		{
			get { return _messageProvider; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_messageProvider = value;
			}
		}
	}
}