using System;
using System.Globalization;
using System.Text;

using NAnt.Core;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	internal class TeamCityMessageProvider : ITeamCityMessageProvider
	{
		readonly TeamCityLogWriter _writer;

		public TeamCityMessageProvider(TeamCityLogWriter writer)
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
			if (String.IsNullOrEmpty(assemblyName))
			{
				return;
			}

			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testSuiteStarted name='{0}']",
			                                Formatter.FormatValue(assemblyName)));
		}

		public void TestSuiteFinished(string assemblyName)
		{
			if (String.IsNullOrEmpty(assemblyName))
			{
				return;
			}

			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testSuiteFinished name='{0}']",
			                                Formatter.FormatValue(assemblyName)));
		}

		public void TestStarted(string testName)
		{
			if (String.IsNullOrEmpty(testName))
			{
				return;
			}

			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testStarted name='{0}']",
			                                Formatter.FormatValue(testName)));
		}

		public void TestIgnored(string testName, string message)
		{
			if (String.IsNullOrEmpty(testName))
			{
				return;
			}

			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testIgnored name='{0}' message='{1}']",
			                                Formatter.FormatValue(testName),
			                                Formatter.FormatValue(message)));
		}

		public void TestFailed(string testName, Exception exception)
		{
			if (String.IsNullOrEmpty(testName))
			{
				return;
			}

			StringBuilder message = new StringBuilder();
			message.AppendFormat(CultureInfo.InvariantCulture,
			                     "##teamcity[testFailed name='{0}'",
			                     Formatter.FormatValue(testName));

			if (exception != null)
			{
				StringBuilder formattedException = new StringBuilder();
				Formatter.FormatException(exception, formattedException);
				Formatter.FormatValue(formattedException);

				message.AppendFormat(CultureInfo.InvariantCulture,
				                     " message='{0}' details='{1}' type='{2}'",
				                     Formatter.FormatValue(exception.Message),
				                     formattedException,
				                     Formatter.FormatValue(exception.GetType().ToString()));
			}

			message.Append("]");

			_writer.WriteLine(message.ToString());
		}

		public void TestOutputStream(string testName, string outputStream)
		{
			if (String.IsNullOrEmpty(testName))
			{
				return;
			}

			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testStdOut name='{0}' out='{1}']",
			                                Formatter.FormatValue(testName),
			                                Formatter.FormatValue(outputStream)));
		}

		public void TestErrorStream(string testName, string errorStream)
		{
			if (String.IsNullOrEmpty(testName))
			{
				return;
			}

			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testStdErr name='{0}' out='{1}']",
			                                Formatter.FormatValue(testName),
			                                Formatter.FormatValue(errorStream)));
		}

		public void TestFinished(string testName)
		{
			if (String.IsNullOrEmpty(testName))
			{
				return;
			}

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