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
		readonly ITeamCityMessageProvider _messageProvider;
		TextWriter _consoleError;
		TextWriter _consoleOut;
		StringWriter _testConsoleError;
		StringWriter _testConsoleOut;

		public TeamCityRunListener(ITeamCityMessageProvider messageProvider)
		{
			if (messageProvider == null)
			{
				throw new ArgumentNullException("messageProvider");
			}

			_messageProvider = messageProvider;
		}

		#region ISpecificationRunListener Members
		public void OnAssemblyStart(Assembly assembly)
		{
			_messageProvider.TestSuiteStarted(assembly.GetName().Name);
		}

		public void OnAssemblyEnd(Assembly assembly)
		{
			_messageProvider.TestSuiteFinished(assembly.GetName().Name);
		}

		public void OnRunStart()
		{
		}

		public void OnRunEnd()
		{
		}

		public void OnSpecificationStart(Specification specification)
		{
			_messageProvider.TestStarted(GetContextSpecName(_currentContext, specification));

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
					_messageProvider.TestFailed(specName, result.Exception);
				}

				string stdStream = _testConsoleOut.ToString();
				if (!String.IsNullOrEmpty(stdStream))
				{
					_messageProvider.TestOutputStream(specName, stdStream);
				}

				string errorStream = _testConsoleError.ToString();
				if (!String.IsNullOrEmpty(errorStream))
				{
					_messageProvider.TestErrorStream(specName, errorStream);
				}

				_messageProvider.TestFinished(specName);
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