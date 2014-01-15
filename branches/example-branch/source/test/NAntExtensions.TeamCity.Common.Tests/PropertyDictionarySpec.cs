using NAntExtensions.ForTesting;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class PropertyDictionarySpec : Spec
	{
		protected const int ExistingValue = int.MinValue + 3;
		protected const string Key = "foo";
		protected const int Value = int.MaxValue;
	}
}