using System.Linq;

using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.Machine.Specifications.Types;

namespace NAntExtensions.Machine.Specifications.Tests
{
	public class When_counters_are_enumerated : Spec
	{
		[Test]
		public void The_All_member_contains_every_public_counter_value()
		{
			Assert.AreEqual(6, Counter.All.Count());
		}

		[Test]
		public void All_counters_are_prefixed_with_mspec()
		{
			foreach (string counter in Counter.All)
			{
				StringAssert.StartsWith(counter, "mspec.");
			}
		}
	}
}