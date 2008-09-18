using NAnt.Core;
using NAnt.Core.Attributes;

using NAntExtensions.TeamCity.Common;

namespace NAntExtensions.MbUnit
{
	[TaskName("mbunit-initcounters")]
	public class InitializeMbUnitCountersTask : Task
	{
		protected override void ExecuteTask()
		{
			PropertyDictionaryHelper.SetInt(Properties, "mbunit.asserts", 0);
			PropertyDictionaryHelper.SetInt(Properties, "mbunit.failures", 0);
			PropertyDictionaryHelper.SetInt(Properties, "mbunit.ignored", 0);
			PropertyDictionaryHelper.SetInt(Properties, "mbunit.run", 0);
			PropertyDictionaryHelper.SetInt(Properties, "mbunit.skipped", 0);
			PropertyDictionaryHelper.SetInt(Properties, "mbunit.successes", 0);
		}
	}
}