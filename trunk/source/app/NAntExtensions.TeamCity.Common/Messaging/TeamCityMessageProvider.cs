using System;
using System.Globalization;
using System.Text;

using NAnt.Core;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	internal class TeamCityMessageProvider : ITeamCityMessageProvider
	{
		readonly IClock _clock;
		readonly TeamCityLogWriter _writer;

		public TeamCityMessageProvider(TeamCityLogWriter writer, Task taskToUseForLogging, IClock clock)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}

			if (taskToUseForLogging == null)
			{
				throw new ArgumentNullException("taskToUseForLogging");
			}

			if (clock == null)
			{
				throw new ArgumentNullException("clock");
			}

			_writer = writer;
			_writer.TaskToUseForLogging = taskToUseForLogging;

			_clock = clock;
		}

		#region ITeamCityMessageProvider Members
		public void TestSuiteStarted(string assemblyName)
		{
			if (String.IsNullOrEmpty(assemblyName))
			{
				return;
			}

			_writer.WriteLine(FormatWithTimestamp("##teamcity[testSuiteStarted name='{0}']", assemblyName));
		}

		public void TestSuiteFinished(string assemblyName)
		{
			if (String.IsNullOrEmpty(assemblyName))
			{
				return;
			}

			_writer.WriteLine(FormatWithTimestamp("##teamcity[testSuiteFinished name='{0}']", assemblyName));
		}

		public void TestStarted(string testName)
		{
			if (String.IsNullOrEmpty(testName))
			{
				return;
			}

			_writer.WriteLine(FormatWithTimestamp("##teamcity[testStarted name='{0}']", testName));
		}

		public void TestIgnored(string testName, string message)
		{
			if (String.IsNullOrEmpty(testName))
			{
				return;
			}

			_writer.WriteLine(FormatWithTimestamp("##teamcity[testIgnored name='{0}' message='{1}']", testName, message));
		}

		public void TestFailed(string testName, Exception exception)
		{
			TestFailed(testName, ExceptionInfo.FromException(exception));
		}

		public void TestFailed(string testName, ExceptionInfo exceptionInfo)
		{
			if (String.IsNullOrEmpty(testName))
			{
				return;
			}

			StringBuilder message = new StringBuilder();
			message.Append(FormatWithTimestamp("##teamcity[testFailed name='{0}'", testName));

			if (exceptionInfo != null)
			{
				StringBuilder formattedException = new StringBuilder();
				Formatter.FormatException(exceptionInfo, formattedException);
				Formatter.FormatValue(formattedException);

				message.AppendFormat(CultureInfo.InvariantCulture,
				                     " message='{0}' details='{1}' type='{2}'",
				                     Formatter.FormatValue(exceptionInfo.Message),
				                     formattedException,
				                     Formatter.FormatValue(exceptionInfo.Type));
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

			_writer.WriteLine(FormatWithTimestamp("##teamcity[testStdOut name='{0}' out='{1}']", testName, outputStream));
		}

		public void TestErrorStream(string testName, string errorStream)
		{
			if (String.IsNullOrEmpty(testName))
			{
				return;
			}

			_writer.WriteLine(FormatWithTimestamp("##teamcity[testStdErr name='{0}' out='{1}']", testName, errorStream));
		}

		public void TestFinished(string testName)
		{
			if (String.IsNullOrEmpty(testName))
			{
				return;
			}

			_writer.WriteLine(FormatWithTimestamp("##teamcity[testFinished name='{0}']", testName));
		}

		public void SendMessage(string message, params object[] parameters)
		{
			if (String.IsNullOrEmpty(message))
			{
				return;
			}

			_writer.WriteLine(FormatWithTimestamp(message, parameters));
		}
		#endregion

		string FormatWithTimestamp(string message, params object[] parameters)
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat(CultureInfo.InvariantCulture, message, Formatter.FormatValues(parameters));

			string formattedResult = builder.ToString();
			if (formattedResult.StartsWith("##teamcity["))
			{
				builder = builder.Insert(builder.Length - 1, " ");
				builder = builder.Insert(builder.Length - 1, CreateTimestamp());
			}

			return builder.ToString();
		}

		/// <summary>
		/// Creates the timestamp for a message.
		/// </summary>
		/// <remarks>Supported by TeamCity 4.0 or greater.</remarks>
		string CreateTimestamp()
		{
			return String.Format("timestamp='{0}'", _clock.Now.ToString("o"));
		}
	}
}