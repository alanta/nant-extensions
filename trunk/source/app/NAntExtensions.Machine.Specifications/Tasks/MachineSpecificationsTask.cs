using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Machine.Specifications.Model;
using Machine.Specifications.Reporting;
using Machine.Specifications.Runner;

using NAnt.Core;
using NAnt.Core.Attributes;
using NAnt.Core.Types;

using NAntExtensions.Machine.Specifications.RunListeners;
using NAntExtensions.TeamCity.Common.BuildEnvironment;
using NAntExtensions.TeamCity.Common.Container;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.Machine.Specifications.Tasks
{
	/// <summary>
	/// Runs Machine.Specifications tests.
	/// </summary>
	/// <example>
	/// <code>
	/// <![CDATA[
	/// <mspec report-directory="reports"
	///	       report-filename="Assembly.Report.html"
	///        workingdir="build-directory"
	///        include-time-info="true"
	///        verbose="true">
	///        <assemblies>
	///            <include name="build-directory/Assembly.dll" />
	///        </assemblies>
	/// </mspec>
	/// ]]></code>
	/// </example>
	[TaskName("mspec")]
	public class MachineSpecificationsTask : Task
	{
		IBuildEnvironment _buildEnvironment;
		DirectoryInfo _workingDirectory;

		/// <summary>
		/// Initializes a new instance of the <see cref="MachineSpecificationsTask"/> class.
		/// </summary>
		public MachineSpecificationsTask() : this(IoC.Resolve<IBuildEnvironment>())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MachineSpecificationsTask"/> class.
		/// </summary>
		/// <param name="buildEnvironment">The build environment.</param>
		public MachineSpecificationsTask(IBuildEnvironment buildEnvironment)
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
		/// The assemblies to include in the test run.
		/// </summary>
		/// <value>The assemblies.</value>
		[BuildElementArray("assemblies", Required = true, ElementType = typeof(FileSet))]
		public FileSet[] Assemblies
		{
			get;
			set;
		}

		/// <summary>
		/// Target output folder for the reports.
		/// </summary>
		/// <value>The report directory.</value>
		[TaskAttribute("report-directory")]
		public string ReportDirectory
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the report file name.
		/// </summary>
		/// <value>The report file name format.</value>
		[TaskAttribute("report-filename")]
		public string ReportFilename
		{
			get;
			set;
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
		/// <c>true</c>, if the time when the report has been generated should be included in the report; otherwise, <c>false</c>.
		/// The default value is <c>false</c>.
		/// </summary>
		[TaskAttribute("include-time-info")]
		[BooleanValidator]
		public bool IncludeTimeInfo
		{
			get;
			set;
		}

		/// <summary>
		/// Executes the task.
		/// </summary>
		protected override void ExecuteTask()
		{
			Log(Level.Info, "Machine.Specifications {0} test runner", typeof(Subject).Assembly.GetName().Version);

			string originalWorkingDirectory = Environment.CurrentDirectory;
			try
			{
				Environment.CurrentDirectory = WorkingDirectory.FullName;

				NAntRunListener nantRunListener = new NAntRunListener(this);

				try
				{
					List<ISpecificationRunListener> listeners = SetUpListeners();
					listeners.Add(nantRunListener);

					AggregateRunListener rootListener = new AggregateRunListener(listeners);
					ISpecificationRunner runner = new AppDomainRunner(rootListener, RunOptions.Default);

					foreach (FileSet assemblySet in Assemblies)
					{
						Log(Level.Info, "Loading {0} assemblies", assemblySet.FileNames.Count);

						foreach (string assemblyName in assemblySet.FileNames)
						{
							Log(Level.Info, "\tAssemblyName: {0}", assemblyName);

							Assembly assembly = Assembly.LoadFrom(assemblyName);
							runner.RunAssembly(assembly);
						}
					}

					Log(Level.Info, "Finished running specs");
				}
				catch (Exception exception)
				{
					Log(Level.Error, exception.ToString());
					throw new BuildException("Specification run failed", exception);
				}

				if (nantRunListener.FailureOccurred)
				{
					if (FailOnError)
					{
						throw new BuildException("Specification run failed");
					}
				}
			}
			finally
			{
				Environment.CurrentDirectory = originalWorkingDirectory;
			}
		}

		List<ISpecificationRunListener> SetUpListeners()
		{
			List<ISpecificationRunListener> result = new List<ISpecificationRunListener>();

			string reportPath = Path.Combine(ReportDirectory, ReportFilename);
			if (!String.IsNullOrEmpty(reportPath))
			{
				GenerateHtmlReportRunListener htmlReport = new GenerateHtmlReportRunListener(reportPath, IncludeTimeInfo);
				result.Add(htmlReport);
			}

			if (BuildEnvironment.IsTeamCityBuild)
			{
				TeamCityRunListener teamCity =
					new TeamCityRunListener(IoC.Resolve<ITeamCityMessageProvider>(new { taskToUseForLogging = this }));

				result.Add(teamCity);
			}

			return result;
		}
	}
}