using MbUnit.Framework;

namespace NAntExtensions.MbUnit.Filtered.Example.Tests.Author
{
	[TestFixture]
	[Author("Tony Tester")]
	public class When_tests_included_by_author_are_run
	{
		[Test]
		public void Should_be_run()
		{
		}
	}
}