using System;
using System.IO;

using MbUnit.Core;
using MbUnit.Core.Filters;
using MbUnit.Core.Graph;
using MbUnit.Core.Reports;
using MbUnit.Core.Reports.Serialization;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using NAntExtensions.TeamCity.Common;
using NAntExtensions.TeamCity.Common.Container;

namespace NAntExtensions.MbUnit
{
	[TaskName("mbunit")]
	public class MbUnitTask : Task
	{
		IBuildEnvironment _buildEnvironment;
		string _reportFileNameFormat = "mbunit-result-{0}{1}";
		string _reportTypes = "Html";
		ReportResult _result;
		string _transformReportFileNameFormat;
		DirectoryInfo _workingDirectory;

		public MbUnitTask() : this(IoC.Resolve<IBuildEnvironment>())
		{
		}

		public MbUnitTask(IBuildEnvironment buildEnvironment)
		{
			BuildEnvironment = buildEnvironment;
		}

		protected IBuildEnvironment BuildEnvironment
		{
			get { return _buildEnvironment; }
			private set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_buildEnvironment = value;
			}
		}

		[TaskAttribute("report-types")]
		public string ReportTypes
		{
			get { return _reportTypes; }
			set { _reportTypes = value; }
		}

		[BuildElementArray("assemblies", Required = true, ElementType = typeof(FileSet))]
		public FileSet[] Assemblies
		{
			get;
			set;
		}

		[BuildElement("assembly-paths", Required = false)]
		public DirSet AssemblyPaths
		{
			get;
			set;
		}

		[TaskAttribute("report-filename-format", Required = false)]
		public string ReportFileNameFormat
		{
			get { return _reportFileNameFormat; }
			set { _reportFileNameFormat = value; }
		}

		[TaskAttribute("report-output-directory", Required = false)]
		public string ReportOutputDirectory
		{
			get;
			set;
		}

		[TaskAttribute("transform", Required = false)]
		public FileInfo Transform
		{
			get;
			set;
		}

		[TaskAttribute("transform-report-filename-format", Required = false)]
		public string TransformReportFileNameFormat
		{
			get
			{
				if (String.IsNullOrEmpty(_transformReportFileNameFormat))
				{
					return ReportFileNameFormat;
				}
				return _transformReportFileNameFormat;
			}
			set { _transformReportFileNameFormat = value; }
		}

		[TaskAttribute("workingdir")]
		public DirectoryInfo WorkingDirectory
		{
			get
			{
				if (_workingDirectory == null)
				{
					return new DirectoryInfo(Environment.CurrentDirectory);
				}
				return _workingDirectory;
			}
			set { _workingDirectory = value; }
		}

		protected override void ExecuteTask()
		{
			Log(Level.Info, "MbUnit {0} test runner", typeof(Fixture).Assembly.GetName().Version);
			DisplayTaskConfiguration();

			if (Assemblies.Length == 0)
			{
				Log(Level.Warning, "No test assemblies, aborting task");
				return;
			}
			int count = 0;
			foreach (FileSet files in Assemblies)
			{
				count += files.FileNames.Count;
			}
			if (count == 0)
			{
				Log(Level.Warning, "No test assemblies found in test");
				return;
			}

			_result = new ReportResult();
			foreach (FileSet assemblySet in Assemblies)
			{
				bool failureCount = ExecuteTests(assemblySet);
				if (failureCount)
				{
					break;
				}
			}

			GenerateReports();

			if (_result.Counter.FailureCount > 0)
			{
				throw new BuildException("There were failing tests. Please see build log.");
			}
		}

		void DisplayTaskConfiguration()
		{
			Log(Level.Verbose, "Test assemblies:");
			foreach (FileSet assemblySet in Assemblies)
			{
				Log(Level.Verbose, "FileSet");
				foreach (string fileName in assemblySet.FileNames)
				{
					Log(Level.Verbose, "\t{0}", fileName);
				}
			}
			Log(Level.Verbose, "ReportTypes: {0}", ReportTypes);
			Log(Level.Verbose, "ReportFileNameFormat: {0}", ReportFileNameFormat);
			Log(Level.Verbose, "ReportOutputDirectory: {0}", ReportOutputDirectory);
		}

		void GenerateReports()
		{
			if (_result == null)
			{
				throw new InvalidOperationException("Report object is a null reference.");
			}

			Log(Level.Info, "Generating reports");
			foreach (string reportType in ReportTypes.Split(';'))
			{
				string reportName = null;
				Log(Level.Verbose, "Report type: {0}", reportType);
				switch (reportType.ToLower())
				{
					case "text":
						reportName = TextReport.RenderToText(_result, ReportOutputDirectory, ReportFileNameFormat);
						break;
					case "xml":
						reportName = XmlReport.RenderToXml(_result, ReportOutputDirectory, ReportFileNameFormat);
						break;
					case "html":
						reportName = HtmlReport.RenderToHtml(_result, ReportOutputDirectory, ReportFileNameFormat);
						break;
					case "dox":
						reportName = DoxReport.RenderToDox(_result, ReportOutputDirectory, ReportFileNameFormat);
						break;
					case "transform":
						if (Transform == null)
						{
							throw new BuildException(String.Format("No transform specified for report type '{0}'", reportType));
						}

						reportName = HtmlReport.RenderToHtml(_result,
						                                     ReportOutputDirectory,
						                                     Transform.FullName,
						                                     TransformReportFileNameFormat);
						break;
					default:
						Log(Level.Error, "Unknown report type {0}", reportType);
						break;
				}

				if (BuildEnvironment.IsTeamCityBuild)
				{
					TeamCityReport.RenderToLog(_result, this);
				}

				if (reportName != null)
				{
					Log(Level.Info, "Created report at {0}", reportName);
				}
			}
		}

		bool ExecuteTests(FileSet assemblySet)
		{
			if (assemblySet.FileNames.Count == 0)
			{
				Log(Level.Warning, "No tests in assembly set");
				return true;
			}
			// execute
			string[] assemblyNames = new string[assemblySet.FileNames.Count];
			assemblySet.FileNames.CopyTo(assemblyNames, 0);

			// display information
			Log(Level.Info, "Loading {0} assemblies", assemblySet.FileNames.Count);
			foreach (string an in assemblyNames)
			{
				Log(Level.Info, "\tAssemblyName: {0}", an);
			}

			string[] dirNames = null;
			if (AssemblyPaths != null)
			{
				dirNames = new string[AssemblyPaths.DirectoryNames.Count];
				AssemblyPaths.DirectoryNames.CopyTo(dirNames, 0);
			}

			try
			{
				using (
					TestDomainDependencyGraph graph = TestDomainDependencyGraph.BuildGraph(assemblyNames,
					                                                                       dirNames,
					                                                                       FixtureFilters.Any,
					                                                                       Verbose))
				{
					graph.Log += Graph_Log;
					try
					{
						string originalWorkingDirectory = Environment.CurrentDirectory;
						Environment.CurrentDirectory = WorkingDirectory.FullName;

						ReportResult r = graph.RunTests();
						Log(Level.Info, "Finished running tests");
						Log(Level.Info, "Merging results");
						_result.Merge(r);
						UpdateNAntProperties(Properties, r);

						Environment.CurrentDirectory = originalWorkingDirectory;
						return r.Counter.FailureCount == 0;
					}
					finally
					{
						graph.Log -= Graph_Log;
					}
				}
			}
			catch (Exception ex)
			{
				throw new BuildException("Unexpected engine error while running tests", ex);
			}
		}

		static void UpdateNAntProperties(PropertyDictionary properties, ReportResult reportResult)
		{
			AddOrUpdate(properties, "mbunit.asserts", reportResult.Counter.AssertCount);
			AddOrUpdate(properties, "mbunit.failures", reportResult.Counter.FailureCount);
			AddOrUpdate(properties, "mbunit.ignored", reportResult.Counter.IgnoreCount);
			AddOrUpdate(properties, "mbunit.run", reportResult.Counter.RunCount);
			AddOrUpdate(properties, "mbunit.skipped", reportResult.Counter.SkipCount);
			AddOrUpdate(properties, "mbunit.successes", reportResult.Counter.SuccessCount);
		}

		static void AddOrUpdate(PropertyDictionary properties, string key, int value)
		{
			int parsedValue;
			if (!int.TryParse(properties[key], out parsedValue))
			{
				parsedValue = 0;
			}

			parsedValue += value;
			properties[key] = parsedValue.ToString();
		}

		void Graph_Log(string message)
		{
			Log(Level.Info, message);
		}
	}
}