using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
	[TaskName("mspec")]
	public class MachineSpecificationsTask : Task
	{
		IBuildEnvironment _buildEnvironment;
		DirectoryInfo _workingDirectory;

		public MachineSpecificationsTask() : this(IoC.Resolve<IBuildEnvironment>())
		{
		}

		public MachineSpecificationsTask(IBuildEnvironment buildEnvironment)
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

		[BuildElementArray("assemblies", Required = true, ElementType = typeof(FileSet))]
		public FileSet[] Assemblies
		{
			get;
			set;
		}

		[TaskAttribute("report-directory")]
		public string ReportDirectory
		{
			get;
			set;
		}

		[TaskAttribute("report-filename")]
		public string ReportFilename
		{
			get;
			set;
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

		[TaskAttribute("include-timing-info")]
		[BooleanValidator]
		public bool IncludeTimeInfo
		{
			get;
			set;
		}

		protected override void ExecuteTask()
		{
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
					ISpecificationRunner runner = new AppDomainRunner(rootListener);

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
					throw new BuildException("Specification run failed");
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
				ITeamCityMessageProvider messageProvider = IoC.Resolve<ITeamCityMessageProvider>();
				messageProvider.Task = this;
				TeamCityRunListener teamCity = new TeamCityRunListener(messageProvider);

				result.Add(teamCity);
			}

			return result;
		}
	}
}