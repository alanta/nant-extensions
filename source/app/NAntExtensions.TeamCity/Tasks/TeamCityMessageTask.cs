using System;

using NAntExtensions.TeamCity.Common;
using NAntExtensions.TeamCity.Common.Container;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.TeamCity.Tasks
{
	public abstract class TeamCityMessageTask : TeamCityTask
	{
		ITeamCityMessageProvider _messageProvider;

		protected TeamCityMessageTask(IBuildEnvironment buildEnvironment) : base(buildEnvironment)
		{
		}

		protected TeamCityMessageTask(IBuildEnvironment buildEnvironment, ITeamCityMessageProvider messageProvider)
			: base(buildEnvironment)
		{
			MessageProvider = messageProvider;
		}

		protected ITeamCityMessageProvider MessageProvider
		{
			get
			{
				if (_messageProvider == null)
				{
					MessageProvider = IoC.Resolve<ITeamCityMessageProvider>(new object[] { new TeamCityLogWriter(this) });
				}
				return _messageProvider;
			}
			private set
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