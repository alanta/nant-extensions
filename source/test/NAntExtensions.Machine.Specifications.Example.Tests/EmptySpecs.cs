#region License
// Copyright (c) 2008 Machine Project
// Portions Copyright (c) 2008 Jacob Lewallen, Aaron Jensen
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
#endregion

using Machine.Specifications;

// This class demonstrates the creation of "empty"
// specs.. technically speaking, this is a normal
// context class in MSpec, with the exception that
// none of the It delegate members are assigned
// an anon method, so they're empty. That being said
// they will be parsed by the runner and still add
// to the test count, but show up as "unimplemented"
// in any reports.
//
// This functionality is entirely optional, as far as
// patterns go, but this is useful for documentating specs
// of some component of the software prior to it's creation,
// ie the UI prior to being designed. This allows
// the implementation team to put expectations down in
// code and have a place to come back to later, when
// the documented functionality is implemented (or
// intra-implementation, even). 

namespace NAntExtensions.Machine.Specifications.Example.Tests
{
	[Subject("Recent Account Activity Summary page")]
	public class when_a_customer_first_views_the_account_summary_page
	{
		It should_display_all_account_transactions_for_the_past_thirty_days;
		It should_display_debit_amounts_in_red_text;
		It should_display_deposit_amounts_in_black_text;
	}
}