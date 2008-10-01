using System;

using Machine.Specifications;

namespace NAntExtensions.Machine.Specifications.Example.Tests
{
	[Subject("mspec NAnt Task")]
	public class When_tests_are_run
	{
		It should_write_some_messages_to_the_console_out_stream = () =>
			{
				Console.Out.WriteLine("Some Console.Out message 1");
				Console.Out.WriteLine("Some Console.Out message 2");
			};

		It should_write_some_messages_to_the_console_error_stream = () =>
		{
			Console.Error.WriteLine("Some Console.Error message 1");
			Console.Error.WriteLine("Some Console.Error message 2");
		};
		
		It should_fail = () =>
		{
			throw new InvalidOperationException("Something bad happened");
		};	
		
		[Ignore]
		It should_be_ignored = () =>
		{
			
		};
	}
}