using System;
using System.Reflection;

using Machine.Specifications.Model;
using Machine.Specifications.Runner;

using NAnt.Core;

using NAntExtensions.TeamCity.Common;

namespace NAntExtensions.Machine.Specifications.RunListeners
{
	public class NAntRunListener : RunListener, ISpecificationRunListener
	{
		readonly Task _task;
		int _contextCount;
		int _failedSpecificationCount;
		int _specificationCount;

		public NAntRunListener(Task task)
		{
			_task = task;

			UpdateNAntProperties(_task.Properties, 0, 0, 0);
		}

		public bool FailureOccurred
		{
			get { return _failedSpecificationCount > 0; }
		}

		#region ISpecificationRunListener Members
		public void OnRunStart()
		{
			_task.Log(Level.Info, "Running specs");

			_contextCount = 0;
			_specificationCount = 0;
			_failedSpecificationCount = 0;
		}

		public void OnRunEnd()
		{
			string summary = String.Format("{0}Contexts: {1}, Specifications: {2}",
			                               Environment.NewLine,
			                               _contextCount,
			                               _specificationCount);
			if (_failedSpecificationCount > 0)
			{
				summary = summary + String.Format(" ({0} failed)", _failedSpecificationCount);
			}

			_task.Log(Level.Info, summary);

			UpdateNAntProperties(_task.Properties, _contextCount, _specificationCount, _failedSpecificationCount);
		}

		public void OnAssemblyStart(Assembly assembly)
		{
			_task.Log(Level.Info, String.Format("{0}Specs in {1}:", Environment.NewLine, assembly.GetName().Name));
		}

		public void OnAssemblyEnd(Assembly assembly)
		{
		}

		public override void OnContextStart(Context context)
		{
			base.OnContextStart(context);

			_task.Log(Level.Verbose, String.Format("{0}{1}", Environment.NewLine, context.FullName));
		}

		public override void OnContextEnd(Context context)
		{
			base.OnContextEnd(context);

			_contextCount++;
		}

		public void OnSpecificationStart(Specification specification)
		{
			_task.Log(Level.Verbose, String.Format("    \x00bb {0}", specification.Name));
		}

		public void OnSpecificationEnd(Specification specification, SpecificationVerificationResult result)
		{
			_specificationCount++;

			if (!result.Passed)
			{
				_failedSpecificationCount++;

				string line = result.Exception.ToString();
				if (!_task.Verbose)
				{
					line = String.Format("{0}{1}{2}",
					                     GetContextSpecNameWithFormat(_currentContext, specification),
					                     Environment.NewLine,
					                     line);
				}

				_task.Log(Level.Error, line);
			}
		}
		#endregion

		static void UpdateNAntProperties(PropertyDictionary properties,
		                                 int contexts,
		                                 int specifications,
		                                 int failedSpecifications)
		{
			PropertyDictionaryHelper.AddOrUpdateInt(properties, "mspec.contexts", contexts);
			PropertyDictionaryHelper.AddOrUpdateInt(properties, "mspec.specs", specifications);
			PropertyDictionaryHelper.AddOrUpdateInt(properties, "mspec.failedspecs", failedSpecifications);
		}
	}
}