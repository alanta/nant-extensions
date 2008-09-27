using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NAntExtensions.Machine.Specifications.Types
{
	public class Counter
	{
		public static string Contexts
		{
			get { return "mspec.contexts"; }
		}

		public static string Specifications
		{
			get { return "mspec.specs"; }
		}

		public static string FailedSpecifications
		{
			get { return "mspec.failedspecs"; }
		}

		public static IEnumerable<string> All
		{
			get
			{
				var foo =
					from prop in typeof(Counter).GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty)
					where typeof(string).IsAssignableFrom(prop.PropertyType)
					select
						typeof(Counter).InvokeMember(prop.Name,
						                             BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Public,
						                             null,
						                             null,
						                             null).ToString();

				return foo;
			}
		}
	}
}