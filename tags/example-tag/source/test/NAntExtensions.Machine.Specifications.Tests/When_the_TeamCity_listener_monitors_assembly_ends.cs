using Machine.Specifications.Runner;

using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.RunListeners;
using NAntExtensions.TeamCity.Common.Messaging;

using Rhino.Mocks;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_the_TeamCity_listener_monitors_assembly_ends : Spec
	{
		AssemblyInfo _assemblyInfo;
		ITeamCityMessageProvider _messageProvider;
		TeamCityRunListener _sut;

		protected override void Before_each_spec()
		{
			_messageProvider = Mocks.StrictMock<ITeamCityMessageProvider>();
			_sut = new TeamCityRunListener(_messageProvider);

			_assemblyInfo = new AssemblyInfo("Assembly");
		}

		[Test]
		public void Should_report_assembly_end_to_TeamCity()
		{
			using (Mocks.Record())
			{
				_messageProvider.TestSuiteFinished(null);
				LastCall.IgnoreArguments();
			}

			using (Mocks.Playback())
			{
				_sut.OnAssemblyEnd(_assemblyInfo);
			}
		}
	}
}