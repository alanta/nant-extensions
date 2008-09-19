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
		TextWriter _consoleError;
		TextWriter _consoleOut;
		StringWriter _testConsoleError;
		StringWriter _testConsoleOut;

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
			_testConsoleError = new StringWriter();
			Console.SetError(_testConsoleError);
		}

		public void OnSpecificationEnd(Specification specification, SpecificationVerificationResult result)
		{
			try
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

				string stdStream = _testConsoleOut.ToString();
				if (!String.IsNullOrEmpty(stdStream))
				{
					_messaging.TestOutputStream(specName, stdStream);
				}

				string errorStream = _testConsoleError.ToString();
				if (!String.IsNullOrEmpty(errorStream))
				{
					_messaging.TestErrorStream(specName, errorStream);
				}

				_messaging.TestFinished(specName);
			}
			finally
			{
				if (_testConsoleOut != null)
				{
					_testConsoleOut.Dispose();
				}

				if (_testConsoleError != null)
				{
					_testConsoleError.Dispose();
				}
			}
		}
		#endregion
	}
}