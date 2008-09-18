using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	public class TeamCityMessaging
	{
		readonly TextWriter _writer;

		public TeamCityMessaging(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}

			_writer = writer;
		}

		#region Test messages
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
		#endregion

		public void Message(string message, params object[] parameters)
		{
			if (String.IsNullOrEmpty(message))
			{
				return;
			}

			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
											String.Format(CultureInfo.InvariantCulture, message, Formatter.FormatValues(parameters))));
		}
	}
}