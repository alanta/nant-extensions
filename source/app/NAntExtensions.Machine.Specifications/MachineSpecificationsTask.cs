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
using NAntExtensions.TeamCity.Common;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.Machine.Specifications
{
	[TaskName("mspec")]
	public class MachineSpecificationsTask : Task
	{
		DirectoryInfo _workingDirectory;

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
					SpecificationRunner runner = new SpecificationRunner(rootListener);

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

				if (nantRunListener.FailureOccured)
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
				GenerateHtmlReportRunListener htmlReport = new GenerateHtmlReportRunListener(reportPath);
				result.Add(htmlReport);
			}

			if (BuildEnvironment.IsTeamCityBuild)
			{
				TeamCityRunListener teamCity =
					new TeamCityRunListener(
						new TeamCityMessaging(new TeamCityLogWriter(this, BuildEnvironment.IsRunningWithTeamCityNAntRunner(this))));
				result.Add(teamCity);
			}

			return result;
		}
	}
}