using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.Machine.Specifications.Types;
using NAntExtensions.TeamCity.Common.Helper;

namespace NAntExtensions.Machine.Specifications.Tasks
{
	[TaskName("mspec-initcounters")]
	public class InitializeMachineSpecificationsCountersTask : Task
	{
		protected override void ExecuteTask()
		{
			foreach (string counter in Counter.All)
			{
				PropertyDictionaryHelper.SetInt(Properties, counter, 0);	
			}
		}
	}
}