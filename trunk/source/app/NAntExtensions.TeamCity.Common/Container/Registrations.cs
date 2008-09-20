using System.Collections.Generic;

using Castle.MicroKernel.Registration;

using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.TeamCity.Common.Container
{
	internal static class Registrations
	{
		internal static IEnumerable<IRegistration> Get()
		{
			yield return Component.For<ITeamCityMessageProvider>().ImplementedBy<TeamCityMessageProvider>();
			yield return Component.For<IEnvironment>().ImplementedBy<SystemEnvironment>();
			yield return Component.For<IBuildEnvironment>().ImplementedBy<BuildEnvironment>();
		}
	}
}