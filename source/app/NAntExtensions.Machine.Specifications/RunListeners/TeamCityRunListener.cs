using System;
using System.IO;

using Machine.Specifications;
using Machine.Specifications.Runner;

using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.Machine.Specifications.RunListeners
{
	internal class TeamCityRunListener : RunListener, ISpecificationRunListener
	{
		readonly ITeamCityMessageProvider _messageProvider;
		TextWriter _defaultConsoleError;
		TextWriter _defaultConsoleOut;
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
		public void OnAssemblyStart(AssemblyInfo assembly)
		{
			_messageProvider.TestSuiteStarted(assembly.Name);
		}

		public void OnAssemblyEnd(AssemblyInfo assembly)
		{
			_messageProvider.TestSuiteFinished(assembly.Name);
		}

		public void OnRunStart()
		{
		}

		public void OnRunEnd()
		{
		}

		public void OnSpecificationStart(SpecificationInfo specification)
		{
			_messageProvider.TestStarted(GetContextSpecName(_currentContext, specification));

			_defaultConsoleOut = Console.Out;
			_testConsoleOut = new StringWriter();
			Console.SetOut(_testConsoleOut);

			_defaultConsoleError = Console.Error;
			_testConsoleError = new StringWriter();
			Console.SetError(_testConsoleError);
		}

		public void OnSpecificationEnd(SpecificationInfo specification, Result result)
		{
			try
			{
				Console.SetOut(_defaultConsoleOut);
				Console.SetError(_defaultConsoleError);
				_testConsoleOut.Flush();
				_testConsoleError.Flush();

				string specName = GetContextSpecName(_currentContext, specification);

				switch (result.Status)
				{
					case Status.Passing:
						break;
					case Status.Failing:
						_messageProvider.TestFailed(specName, result.Exception);
						break;
					case Status.Ignored:
						_messageProvider.TestIgnored(specName, null);
						break;
					case Status.NotImplemented:
						_messageProvider.TestIgnored(specName, "Not implemented");
						break;
					default:
						break;
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