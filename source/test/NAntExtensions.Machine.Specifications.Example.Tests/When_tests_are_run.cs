using System;

using Machine.Specifications;

namespace NAntExtensions.Machine.Specifications.Example.Tests
{
	[Subject("mspec NAnt Task")]
	public class When_tests_are_run
	{
		It is_not_implemented;

		[Ignore]
		It should_be_ignored = () => { };

		It should_fail = () =>
			{
				Console.Out.WriteLine("Some Console.Out message");
				Console.Error.WriteLine("Some Console.Error message");
				throw new InvalidOperationException("Something bad happened");
			};

		It should_succeed = () => { };
	}
}