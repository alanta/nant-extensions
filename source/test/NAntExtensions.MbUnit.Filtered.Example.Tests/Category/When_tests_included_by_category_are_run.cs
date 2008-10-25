using MbUnit.Framework;

namespace NAntExtensions.MbUnit.Filtered.Example.Tests.Category
{
	[TestFixture]
	[FixtureCategory("Include")]
	public class When_tests_included_by_category_are_run
	{
		[Test]
		public void Should_be_run()
		{
		}
	}
}