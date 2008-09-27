using System.Collections.Generic;
using System.Reflection;

namespace NAntExtensions.MbUnit.Types
{
	public class Counter
	{
		public const string Asserts = "mbunit.asserts";
		public const string Failures = "mbunit.failures";
		public const string Ignored = "mbunit.ignored";
		public const string Run = "mbunit.run";
		public const string Skipped = "mbunit.skipped";
		public const string Successes = "mbunit.successes";

		public static IEnumerable<string> All
		{
			get
			{
				List<string> result = new List<string>();

				foreach (
					FieldInfo field in typeof(Counter).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField))
				{
					if (!typeof(string).IsAssignableFrom(field.FieldType))
					{
						continue;
					}

					result.Add(
						typeof(Counter).InvokeMember(field.Name,
						                             BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public,
						                             null,
						                             null,
						                             null).ToString());
				}

				return result;
			}
		}
	}
}