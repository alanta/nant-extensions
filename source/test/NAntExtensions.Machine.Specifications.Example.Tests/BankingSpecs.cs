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

using System;

using Machine.Specifications;

namespace NAntExtensions.Machine.Specifications.Example.Tests
{
	[Subject(typeof(Account), "Funds transfer")]
	public class when_transferring_between_two_accounts : with_from_account_and_to_account
	{
		Because of = () => fromAccount.Transfer(1m, toAccount);

		It should_credit_the_to_account_by_the_amount_transferred = () => toAccount.Balance.ShouldEqual(2m);
		It should_debit_the_from_account_by_the_amount_transferred = () => fromAccount.Balance.ShouldEqual(0m);
	}

	[Subject(typeof(Account), "Funds transfer")]
	public class when_transferring_an_amount_larger_than_the_balance_of_the_from_account : with_from_account_and_to_account
	{
		static Exception exception;
		Because of = () => exception = Catch.Exception(() => fromAccount.Transfer(2m, toAccount));

		It should_not_allow_the_transfer = () => exception.ShouldBeOfType<Exception>();
	}

	public abstract class with_from_account_and_to_account
	{
		protected static Account fromAccount;
		protected static Account toAccount;

		Establish context = () =>
			{
				fromAccount = new Account { Balance = 1m };
				toAccount = new Account { Balance = 1m };
			};
	}
}