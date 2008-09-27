using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.MbUnit.Types;
using NAntExtensions.TeamCity.Common.Helper;

namespace NAntExtensions.MbUnit.Tasks
{
	[TaskName("mbunit-initcounters")]
	public class InitializeMbUnitCountersTask : Task
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