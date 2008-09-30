using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_the_TeamCity_listener_monitors_run_ends : Spec
	{
		ITeamCityMessageProvider _messageProvider;
		TeamCityRunListener _sut;

		protected override void Before_each_spec()
		{
			_messageProvider = Mocks.StrictMock<ITeamCityMessageProvider>();
			_sut = new TeamCityRunListener(_messageProvider);
		}

		[Test]
		public void Does_not_report_run_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
			}

			using (Mocks.Playback())
			{
				_sut.OnRunEnd();
			}
		}
	}
}