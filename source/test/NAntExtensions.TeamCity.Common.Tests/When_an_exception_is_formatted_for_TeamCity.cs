using System;
using System.Text;

using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_an_exception_is_formatted_for_TeamCity : Spec
	{
		StringBuilder _builder;

		protected override void Before_each_spec()
		{
			_builder = new StringBuilder();
		}

		[Test]
		public void Does_not_alter_result_string_if_exception_is_null()
		{
			Formatter.FormatException(null, _builder);

			StringAssert.IsEmpty(_builder.ToString());
		}

		[Test]
		public void Can_format_a_single_exception()
		{
			Exception exception = new Exception("foo");
			exception.Source = "here";

			Formatter.FormatException(ExceptionInfo.FromException(exception), _builder);

			StringAssert.Contains(_builder.ToString(), "foo");
		}

		[Test]
		public void Can_format_nested_exceptions()
		{
			Formatter.FormatException(ExceptionInfo.FromException(new Exception("foo", new InvalidOperationException("bar"))),
			                          _builder);

			StringAssert.Contains(_builder.ToString(), "foo");
			StringAssert.Contains(_builder.ToString(), "bar");
		}

		[Test]
		public void Prepends_separator_if_message_is_not_empty()
		{
			_builder.Append("start");

			Formatter.FormatException(ExceptionInfo.FromException(new Exception("foo")), _builder);

			StringAssert.StartsWith(_builder.ToString(), "start-");
		}
	}
}