using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NAntExtensions.Machine.Specifications.Types
{
	internal class Counter
	{
		public const string Contexts = "mspec.contexts";
		public const string FailedSpecifications = "mspec.failedspecs";
		public const string Specifications = "mspec.specs";

		public static IEnumerable<string> All
		{
			get
			{
				var values =
					from field in typeof(Counter).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField)
					where typeof(string).IsAssignableFrom(field.FieldType)
					select
						typeof(Counter).InvokeMember(field.Name,
						                             BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public,
						                             null,
						                             null,
						                             null).ToString();

				return values;
			}
		}
	}
}