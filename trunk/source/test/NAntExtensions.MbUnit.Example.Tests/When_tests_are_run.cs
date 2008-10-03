using System;
using System.Threading;

using MbUnit.Framework;

namespace NAntExtensions.MbUnit.Example.Tests
{
	[TestFixture]
	public class When_tests_are_run
	{
		[Test]
		public void Should_succeed()
		{
		}
		
		[Test]
		public void Should_run_very_long()
		{
			Thread.Sleep(1500);
		}

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

		[Test]
		[Explicit("Some reason")]
		public void Should_be_ignored_because_the_test_is_marked_as_explicit_with_message()
		{
		}

		[Test]
		[Ignore("Some reason")]
		public void Should_be_ignored_with_message()
		{
		}
	}
}