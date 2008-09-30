using Machine.Specifications.Runner;

using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;
using NAntExtensions.TeamCity.Common.Messaging;

using Rhino.Mocks;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_the_TeamCity_listener_monitors_specification_starts : Spec
	{
		ContextInfo _contextInfo;
		ITeamCityMessageProvider _messageProvider;
		SpecificationInfo _specificationInfo;
		TeamCityRunListener _sut;

		protected override void Before_each_spec()
		{
			_messageProvider = Mocks.StrictMock<ITeamCityMessageProvider>();
			_sut = new TeamCityRunListener(_messageProvider);

			_contextInfo = new ContextInfo("Context", "Concern");
			_specificationInfo = new SpecificationInfo("Spec");
		}

		[Test]
		public void Should_report_specification_start_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestStarted(null);
				LastCall.IgnoreArguments();
			}

			_sut.OnContextStart(_contextInfo);

			using (Mocks.Playback())
			{
				_sut.OnSpecificationStart(_specificationInfo);
			}
		}
	}
}