using System;

using MbUnit.Framework;

namespace NAntExtensions.MbUnit.Example.Tests
{
	[TestFixture]
	public class When_tests_are_run
	{
		[Test]
		public void Should_write_some_messages_to_the_console_out_stream()
		{
			Console.Out.WriteLine("Some Console.Out message 1");
			Console.Out.WriteLine("Some Console.Out message 2");
		}

		[Test]
		public void Should_write_some_messages_to_the_console_error_stream()
		{
			Console.Error.WriteLine("Some Console.Error message 1");
			Console.Error.WriteLine("Some Console.Error message 2");
		}

		[Test]
		public void Should_fail()
		{
			throw new InvalidOperationException("Something bad happened");
		}
		
		[Explicit]
		public void Should_be_ignored_because_the_test_is_marked_as_explicit()
		{
		}
		
		[Ignore]
		public void Should_be_ignored()
		{
		}
	}
}