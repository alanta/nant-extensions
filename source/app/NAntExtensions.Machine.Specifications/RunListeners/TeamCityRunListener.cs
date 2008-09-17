using System;
using System.Reflection;

using Machine.Specifications.Model;
using Machine.Specifications.Runner;

using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.Machine.Specifications.RunListeners
{
	internal class TeamCityRunListener : RunListener, ISpecificationRunListener
	{
		readonly TeamCityMessaging _messaging;

		public TeamCityRunListener(TeamCityMessaging messaging)
		{
			if (messaging == null)
			{
				throw new ArgumentNullException("messaging");
			}

			_messaging = messaging;
		}

		#region ISpecificationRunListener Members
		public void OnAssemblyStart(Assembly assembly)
		{
			_messaging.TestSuiteStarted(assembly.GetName().Name);
		}

		public void OnAssemblyEnd(Assembly assembly)
		{
			_messaging.TestSuiteFinished(assembly.GetName().Name);
		}

		public void OnRunStart()
		{
		}

		public void OnRunEnd()
		{
		}

		public void OnSpecificationStart(Specification specification)
		{
			_messaging.TestStarted(GetContextSpecName(_currentContext, specification));
		}

		public void OnSpecificationEnd(Specification specification, SpecificationVerificationResult result)
		{
			string specName = GetContextSpecName(_currentContext, specification);

			if (!result.Passed)
			{
				_messaging.TestFailed(specName, result.Exception);
			}

			// TODO: Read contents of stdout and stderr and report to TeamCity.

			_messaging.TestFinished(specName);
		}
		#endregion
	}
}