using MbUnit.Framework;

namespace NAntExtensions.MbUnit.Filtered.Example.Tests.Category
{
	[TestFixture]
	[FixtureCategory("Exclude")]
	public class When_tests_excluded_by_category_are_run
	{
		[Test]
		public void Should_be_excluded()
		{
			Assert.Fail("This test should have been excluded from the test run");
		}
	}
}