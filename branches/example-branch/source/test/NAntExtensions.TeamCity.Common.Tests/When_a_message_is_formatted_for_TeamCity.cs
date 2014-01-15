using System;
using System.Text;

using MbUnit.Framework;

using NAntExtensions.ForTesting;
using NAntExtensions.TeamCity.Common.Messaging;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_a_message_is_formatted_for_TeamCity : Spec
	{
		StringBuilder _builder;

		protected override void Before_each_spec()
		{
			_builder = new StringBuilder();
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Throws_if_FormatValue_is_called_with_a_null_StringBuilder()
		{
			Formatter.FormatValue((StringBuilder) null);
		}

		[Test]
		public void Should_encode_null_values_to_empty_strings()
		{
			Assert.IsEmpty(Formatter.FormatValue((string) null));
		}

		[Test]
		public void Should_encode_the_pipe_character()
		{
			_builder.Append("|");

			Formatter.FormatValue(_builder);

			Assert.AreEqual("||", _builder.ToString());
		}

		[Test]
		public void Should_encode_the_apostrophe_character()
		{
			_builder.Append("'");

			Formatter.FormatValue(_builder);

			Assert.AreEqual("|'", _builder.ToString());
		}

		[Test]
		public void Should_encode_the_newline_character()
		{
			_builder.Append("\n");

			Formatter.FormatValue(_builder);

			Assert.AreEqual("|n", _builder.ToString());
		}

		[Test]
		public void Should_encode_the_carriage_return_character()
		{
			_builder.Append("\r");

			Formatter.FormatValue(_builder);

			Assert.AreEqual("|r", _builder.ToString());
		}

		[Test]
		public void Should_encode_the_closing_square_bracket_character()
		{
			_builder.Append("]");

			Formatter.FormatValue(_builder);

			Assert.AreEqual("|]", _builder.ToString());
		}

		[Test]
		public void Should_encode_special_characters_when_passing_a_string()
		{
			Assert.AreEqual("|||'|n|r|]", Formatter.FormatValue("|'\n\r]"));
		}
	}
}