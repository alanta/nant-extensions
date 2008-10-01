using System;

using MbUnit.Framework;

namespace NAntExtensions.MbUnit.Example.Tests
{
	[TestFixture]
	public class When_tests_are_run
	{
		[Test]
		public void Should_fail()
		{
			Console.Out.WriteLine("Some Console.Out message");
			Console.Error.WriteLine("Some Console.Error message");
			throw new InvalidOperationException("Something bad happened");
		}

		[Test]
		[Explicit]
		public void Should_be_ignored_because_the_test_is_marked_as_explicit()
		{
		}

		[Test]
		[Ignore]
		public void Should_be_ignored()
		{
		}
	}
}