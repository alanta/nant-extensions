using System;
using System.Globalization;
using System.IO;
using System.Text;

using MbUnit.Core.Reports;
using MbUnit.Core.Reports.Serialization;

using NAnt.Core;

using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Container;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.MbUnit
{
	// TODO: Refactor to make use of NAntExtensions.TeamCity.Common.TeamCityMessenger.
	public class TeamCityReport : ReportBase
	{
		protected override string DefaultExtension
		{
			get { return ".txt"; }
		}

		public override void Render(ReportResult result, TextWriter writer)
		{
			foreach (ReportAssembly reportAssembly in result.Assemblies)
			{
				writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
				                               "##teamcity[testSuiteStarted name='{0}']",
				                               FormatValue(reportAssembly.Name)));
				foreach (ReportNamespace reportNamespace in reportAssembly.Namespaces)
				{
					RecurseNameSpaces(reportNamespace, writer);
				}
				writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
				                               "##teamcity[testSuiteFinished name='{0}']",
				                               FormatValue(reportAssembly.Name)));
			}
		}

		static void RecurseNameSpaces(ReportNamespace reportNamespace, TextWriter writer)
		{
			foreach (ReportFixture reportFixture in reportNamespace.Fixtures)
			{
				foreach (ReportRun run in reportFixture.Runs)
				{
					string formattedRunName = FormatValue(run.Name);
					writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "##teamcity[testStarted name='{0}']", formattedRunName));
					if (run.Result == ReportRunResult.Ignore || run.Result == ReportRunResult.Skip)
					{
						writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
						                               "##teamcity[testIgnored name='{0}' message='{1}']",
						                               formattedRunName,
						                               run.Result));
					}
					else if (run.Result == ReportRunResult.Failure)
					{
						StringBuilder formattedException = new StringBuilder();
						FormatException(run.Exception, formattedException);
						FormatValue(formattedException);
						//type='comparisonFailure' name='testname' message='failure message' details='stack trace' expected='expected value' actual='actual value'
						writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
						                               "##teamcity[testFailed  name='{0}' message='{1}' details='{2}' type='{3}']",
						                               formattedRunName,
						                               FormatValue(run.Exception.Message),
						                               formattedException,
						                               run.Exception.Type));
					}
					if (!string.IsNullOrEmpty(run.ConsoleOut))
					{
						writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
						                               "##teamcity[testStdOut name='{0}' out='{1}']",
						                               formattedRunName,
						                               FormatValue(run.ConsoleOut)));
					}
					if (!string.IsNullOrEmpty(run.ConsoleError))
					{
						writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
						                               "##teamcity[testStdErr name='{0}' out='{1}']",
						                               formattedRunName,
						                               FormatValue(run.ConsoleError)));
					}

					writer.WriteLine(string.Format(CultureInfo.InvariantCulture,
					                               "##teamcity[testFinished name='{0}']",
					                               formattedRunName));
				}
			}

			foreach (ReportNamespace child in reportNamespace.Namespaces)
			{
				RecurseNameSpaces(child, writer);
			}
		}

		static void FormatException(ReportException exception, StringBuilder sb)
		{
			if (null != exception)
			{
				if (sb.Length > 0)
				{
					sb.AppendLine("--------------------------------");
				}
				sb.AppendFormat("{0} : {1}", exception.Type, exception.Message);
				sb.AppendLine();
				if (!string.IsNullOrEmpty(exception.Source))
				{
					sb.AppendFormat(" Source: {0}", exception.Source);
				}
				sb.AppendLine();
				sb.AppendFormat(" Stack: {0}", exception.StackTrace);
				sb.AppendLine();

				if (exception.Exception != null)
				{
					FormatException(exception.Exception, sb);
				}
			}
		}

		static string FormatValue(string value)
		{
			StringBuilder sb = new StringBuilder(value);
			FormatValue(sb);
			return sb.ToString();
		}

		static void FormatValue(StringBuilder sb)
		{
			sb.Replace("|", "||");
			sb.Replace("'", "|'");
			sb.Replace("\n", "|n");
			sb.Replace("\r", "|r");
			sb.Replace("]", "|]");
		}

		internal static void RenderToLog(ReportResult result, Task task)
		{
			if (null == result)
			{
				throw new ArgumentNullException("result");
			}

			if (null == task)
			{
				throw new ArgumentNullException("task");
			}

			TeamCityReport tcReport = new TeamCityReport();
			// TODO: this might break.
			using (TextWriter writer = IoC.Resolve<ITeamCityLogWriter>() as TextWriter)
			{
				tcReport.Render(result, writer);
			}
		}
	}
}