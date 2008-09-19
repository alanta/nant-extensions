using System;
using System.IO;
using System.Reflection;

using Machine.Specifications.Model;
using Machine.Specifications.Runner;

using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.Machine.Specifications.RunListeners
{
	internal class TeamCityRunListener : RunListener, ISpecificationRunListener
	{
		readonly ITeamCityMessaging _messaging;
		TextWriter _consoleOut;
		StringWriter _testConsoleOut;
		TextWriter _consoleError;
		StringWriter _testConsoleError;

		public TeamCityRunListener(ITeamCityMessaging messaging)
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

			_consoleOut = Console.Out;
			_testConsoleOut = new StringWriter();
			Console.SetOut(_testConsoleOut);


			_consoleError = Console.Error;
			_testConsoleError= new StringWriter();
			Console.SetError(_testConsoleError);
		}

		public void OnSpecificationEnd(Specification specification, SpecificationVerificationResult result)
		{
			Console.SetOut(_consoleOut);
			Console.SetError(_consoleError);
			_testConsoleOut.Flush();
			_testConsoleError.Flush();
			
			string specName = GetContextSpecName(_currentContext, specification);

			if (!result.Passed)
			{
				_messaging.TestFailed(specName, result.Exception);
			}

			_messaging.TestOutputStream(specName, _testConsoleOut.ToString());
			_messaging.TestOutputStream(specName, _testConsoleError.ToString());

			_messaging.TestFinished(specName);
		}
		#endregion
	}
}