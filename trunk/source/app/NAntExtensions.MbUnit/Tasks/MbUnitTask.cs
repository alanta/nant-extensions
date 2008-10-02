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

using NAntExtensions.MbUnit.Reporting;
using NAntExtensions.MbUnit.Types;
using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Container;
using NAntExtensions.TeamCity.Common.Helper;

namespace NAntExtensions.MbUnit.Tasks
{
	/// <summary>
	/// Runs MbUnit tests.
	/// </summary>
	/// <remarks>
	/// If running within a TeamCity build, the test results are reported to TeamCity.
	/// Note that due to the way MbUnit logs its test results the timing information for the 
	/// tests shown by TeamCity is not valid.</remarks>
	/// <example>
	/// <code>
	/// <![CDATA[
	/// <mbunit report-types="html"
	///         report-filename-format="Assembly.Report"
	///         report-directory="reports"
	///         verbose="true"
	///         workingdir="build-directory">
	///         <assemblies>
	///             <include name="build-directory/Assembly.dll" />
	///         </assemblies>
	/// </mbunit>
	/// ]]></code>
	/// </example>
	[TaskName("mbunit")]
	public class MbUnitTask : Task
	{
		IBuildEnvironment _buildEnvironment;
		string _reportFileNameFormat = "mbunit-{0}{1}";
		string _reportTypes = "Html";
		ReportResult _result;
		string _transformReportFileNameFormat;
		DirectoryInfo _workingDirectory;

		/// <summary>
		/// Initializes a new instance of the <see cref="MbUnitTask"/> class.
		/// </summary>
		public MbUnitTask() : this(IoC.Resolve<IBuildEnvironment>())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MbUnitTask"/> class.
		/// </summary>
		/// <param name="buildEnvironment">The build environment.</param>
		public MbUnitTask(IBuildEnvironment buildEnvironment)
		{
			BuildEnvironment = buildEnvironment;
		}

		IBuildEnvironment BuildEnvironment
		{
			get { return _buildEnvironment; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				_buildEnvironment = value;
			}
		}

		/// <summary>
		/// Gets or sets the report format to generate. Valid values are: text, xml, dox, html and transform. If you specify
		/// transform, you will also have to set the <see cref="Transform"/> attribute. You can set multiple values separated by a semicolon (;).
		/// </summary>
		/// <value>The report types.</value>
		[TaskAttribute("report-types")]
		public string ReportTypes
		{
			get { return _reportTypes; }
			set { _reportTypes = value; }
		}

		/// <summary>
		/// Gets or sets the assemblies to include in the test run.
		/// </summary>
		/// <value>The assemblies.</value>
		[BuildElementArray("assemblies", Required = true, ElementType = typeof(FileSet))]
		public FileSet[] Assemblies
		{
			get;
			set;
		}

		/// <summary>
		/// Path where the assemblies can be loaded.
		/// </summary>
		/// <value>The assembly paths.</value>
		[BuildElement("assembly-paths", Required = false)]
		public DirSet AssemblyPaths
		{
			get;
			set;
		}

		/// <summary>
		/// The report file name format for all reports except the 'transform' report. The default value is 'mbunit-result-{0}{1}'
		/// where {0} is replaced by the date and {1} is replaced by the time.
		/// </summary>
		/// <value>The report file name format.</value>
		[TaskAttribute("report-filename-format", Required = false)]
		public string ReportFileNameFormat
		{
			get { return _reportFileNameFormat; }
			set { _reportFileNameFormat = value; }
		}

		/// <summary>
		/// Target output folder for the reports.
		/// </summary>
		/// <value>The report directory.</value>
		[TaskAttribute("report-directory", Required = false)]
		public string ReportDirectory
		{
			get;
			set;
		}

		/// <summary>
		/// The XSL transformation file to use for the 'transform' report.
		/// </summary>
		/// <value>The transform.</value>
		[TaskAttribute("transform", Required = false)]
		public FileInfo Transform
		{
			get;
			set;
		}

		/// <summary>
		/// The report file name format to use for the 'transform' report. If you do not specify this value, the value from 
		/// <see cref="ReportFileNameFormat"/> is used.
		/// </summary>
		/// <value>The transform report file name format.</value>
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

		/// <summary>
		/// The directory in which the test run will be executed. If you do not specify this value, the current directory is used.
		/// </summary>
		/// <value>The working directory.</value>
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

		/// <summary>
		/// Executes the task.
		/// </summary>
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
				foreach (string fileName in assemblySet.FileNames)
				{
					Log(Level.Verbose, "\t{0}", fileName);
				}
			}
			Log(Level.Verbose, "Working directory: {0}", WorkingDirectory);
			Log(Level.Verbose, "Report types: {0}", ReportTypes);
			Log(Level.Verbose, "Report directory: {0}", ReportDirectory);
			Log(Level.Verbose, "Report file name format: {0}", ReportFileNameFormat);
			Log(Level.Verbose, "Transform: {0}", Transform == null ? String.Empty : Transform.FullName);
			Log(Level.Verbose, "Transform report file name format: {0}", TransformReportFileNameFormat);
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
						reportName = TextReport.RenderToText(_result, ReportDirectory, ReportFileNameFormat);
						break;
					case "xml":
						reportName = XmlReport.RenderToXml(_result, ReportDirectory, ReportFileNameFormat);
						break;
					case "html":
						reportName = HtmlReport.RenderToHtml(_result, ReportDirectory, ReportFileNameFormat);
						break;
					case "dox":
						reportName = DoxReport.RenderToDox(_result, ReportDirectory, ReportFileNameFormat);
						break;
					case "transform":
						if (Transform == null)
						{
							throw new BuildException(String.Format("No transform specified for report type '{0}'", reportType));
						}

						reportName = HtmlReport.RenderToHtml(_result, ReportDirectory, Transform.FullName, TransformReportFileNameFormat);
						break;
					default:
						Log(Level.Error, "Unknown report type {0}", reportType);
						break;
				}

				if (BuildEnvironment.IsTeamCityBuild)
				{
					TeamCityReportGenerator.RenderReport(_result, this);
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
			PropertyDictionaryHelper.AddOrUpdateInt(properties, Counter.Asserts, reportResult.Counter.AssertCount);
			PropertyDictionaryHelper.AddOrUpdateInt(properties, Counter.Failures, reportResult.Counter.FailureCount);
			PropertyDictionaryHelper.AddOrUpdateInt(properties, Counter.Ignored, reportResult.Counter.IgnoreCount);
			PropertyDictionaryHelper.AddOrUpdateInt(properties, Counter.Run, reportResult.Counter.RunCount);
			PropertyDictionaryHelper.AddOrUpdateInt(properties, Counter.Skipped, reportResult.Counter.SkipCount);
			PropertyDictionaryHelper.AddOrUpdateInt(properties, Counter.Successes, reportResult.Counter.SuccessCount);
		}

		void Graph_Log(string message)
		{
			Log(Level.Info, message);
		}
	}
}