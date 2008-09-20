using System;
using System.Globalization;
using System.Text;

using NAnt.Core;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	internal class TeamCityMessageProvider : ITeamCityMessageProvider
	{
		readonly ITeamCityLogWriter _writer;

		public TeamCityMessageProvider(ITeamCityLogWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}

			_writer = writer;
		}

		#region ITeamCityMessageProvider Members
		public Task Task
		{
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				_writer.Task = value;
			}
		}

		public void TestSuiteStarted(string assemblyName)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testSuiteStarted name='{0}']",
			                                Formatter.FormatValue(assemblyName)));
		}

		public void TestSuiteFinished(string assemblyName)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testSuiteFinished name='{0}']",
			                                Formatter.FormatValue(assemblyName)));
		}

		public void TestStarted(string testName)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testStarted name='{0}']",
			                                Formatter.FormatValue(testName)));
		}

		public void TestIgnored(string testName, string message)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testIgnored name='{0}' message='{1}']",
			                                Formatter.FormatValue(testName),
			                                Formatter.FormatValue(message)));
		}

		public void TestFailed(string testName, Exception exception)
		{
			StringBuilder formattedException = new StringBuilder();
			Formatter.FormatException(exception, formattedException);
			Formatter.FormatValue(formattedException);

			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testFailed  name='{0}' message='{1}' details='{2}' type='{3}']",
			                                Formatter.FormatValue(testName),
			                                Formatter.FormatValue(exception.Message),
			                                formattedException,
			                                Formatter.FormatValue(exception.GetType().ToString())));
		}

		public void TestOutputStream(string testName, string outputStream)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testStdOut name='{0}' out='{1}']",
			                                Formatter.FormatValue(testName),
			                                Formatter.FormatValue(outputStream)));
		}

		public void TestErrorStream(string testName, string errorStream)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testStdErr name='{0}' out='{1}']",
			                                Formatter.FormatValue(testName),
			                                Formatter.FormatValue(errorStream)));
		}

		public void TestFinished(string testName)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testFinished name='{0}']",
			                                Formatter.FormatValue(testName)));
		}

		public void SendMessage(string message, params object[] parameters)
		{
			if (String.IsNullOrEmpty(message))
			{
				return;
			}

			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture, message, Formatter.FormatValues(parameters)));
		}
		#endregion
	}
}