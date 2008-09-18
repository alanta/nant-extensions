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
			                                FormatValue(assemblyName)));
		}

		public void TestSuiteFinished(string assemblyName)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testSuiteFinished name='{0}']",
			                                FormatValue(assemblyName)));
		}

		public void TestStarted(string testName)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testStarted name='{0}']",
			                                FormatValue(testName)));
		}

		public void TestIgnored(string testName, string message)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testIgnored name='{0}' message='{1}']",
			                                FormatValue(testName),
			                                FormatValue(message)));
		}

		public void TestFailed(string testName, Exception exception)
		{
			StringBuilder formattedException = new StringBuilder();
			FormatException(exception, formattedException);
			FormatValue(formattedException);

			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testFailed  name='{0}' message='{1}' details='{2}' type='{3}']",
			                                FormatValue(testName),
			                                FormatValue(exception.Message),
			                                formattedException,
			                                FormatValue(exception.GetType().ToString())));
		}

		public void TestOutputStream(string testName, string outputStream)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testStdOut name='{0}' out='{1}']",
			                                FormatValue(testName),
			                                FormatValue(outputStream)));
		}

		public void TestErrorStream(string testName, string errorStream)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testStdErr name='{0}' out='{1}']",
			                                FormatValue(testName),
			                                FormatValue(errorStream)));
		}

		public void TestFinished(string testName)
		{
			_writer.WriteLine(String.Format(CultureInfo.InvariantCulture,
			                                "##teamcity[testFinished name='{0}']",
			                                FormatValue(testName)));
		}
		#endregion

		static void FormatException(Exception exception, StringBuilder builder)
		{
			if (exception == null)
			{
				return;
			}

			if (builder.Length > 0)
			{
				builder.AppendLine("--------------------------------");
			}

			builder.AppendFormat("{0}: {1}", exception.GetType(), exception.Message);
			builder.AppendLine();

			if (!String.IsNullOrEmpty(exception.Source))
			{
				builder.AppendFormat(" Source: {0}", exception.Source);
			}
			builder.AppendLine();

			builder.AppendFormat(" Stack: {0}", exception.StackTrace);
			builder.AppendLine();

			if (exception.InnerException != null)
			{
				FormatException(exception.InnerException, builder);
			}
		}

		static string FormatValue(string value)
		{
			StringBuilder sb = new StringBuilder(value);
			FormatValue(sb);

			return sb.ToString();
		}

		static void FormatValue(StringBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException("builder");
			}

			builder.Replace("|", "||");
			builder.Replace("'", "|'");
			builder.Replace("\n", "|n");
			builder.Replace("\r", "|r");
			builder.Replace("]", "|]");
		}
	}
}