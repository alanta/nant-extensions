using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common.Helper;

namespace NAntExtensions.Machine.Specifications.Tasks
{
	[TaskName("mspec-initcounters")]
	public class InitializeMachineSpecificationsCountersTask : Task
	{
		protected override void ExecuteTask()
		{
			PropertyDictionaryHelper.SetInt(Properties, "mspec.contexts", 0);
			PropertyDictionaryHelper.SetInt(Properties, "mspec.specs", 0);
			PropertyDictionaryHelper.SetInt(Properties, "mspec.failedspecs", 0);
		}
	}
}