using System;
using System.Globalization;

using MbUnit.Framework;

using NAnt.Core;

using NAntExtensions.TeamCity.Common.Helper;

namespace NAntExtensions.TeamCity.Common.Tests
{
	public class When_properties_are_set : PropertyDictionarySpec
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Throws_exception_if_properties_are_null()
		{
			PropertyDictionaryHelper.SetInt(null, null, Value);
		}

		[RowTest]
		[Row(SpecialValue.Null, ExpectedException = typeof(ArgumentOutOfRangeException))]
		[Row("", ExpectedException = typeof(ArgumentOutOfRangeException))]
		public void Throws_exception_if_key_is_null_or_empty(string key)
		{
			PropertyDictionaryHelper.SetInt(new PropertyDictionary(null), key, Value);
		}

		[Test]
		public void Adds_key_if_key_does_not_exists()
		{
			PropertyDictionary properties = new PropertyDictionary(null);

			PropertyDictionaryHelper.SetInt(properties, Key, Value);

			Assert.IsNotNull(properties[Key]);
		}

		[Test]
		public void Adds_value_by_key_if_key_does_not_exists()
		{
			PropertyDictionary properties = new PropertyDictionary(null);

			PropertyDictionaryHelper.SetInt(properties, Key, Value);

			Assert.AreEqual(int.Parse(properties[Key], CultureInfo.InvariantCulture), Value);
		}
		
		[Test]
		public void Replaces_key_if_key_does_exists()
		{
			PropertyDictionary properties = new PropertyDictionary(null);
			properties.Add(Key, ExistingValue.ToString());

			PropertyDictionaryHelper.SetInt(properties, Key, Value);

			Assert.IsNotNull(properties[Key]);
		}

		[Test]
		public void Replaces_value_by_key_if_key_does_exists()
		{
			PropertyDictionary properties = new PropertyDictionary(null);
			properties.Add(Key, ExistingValue.ToString());

			PropertyDictionaryHelper.SetInt(properties, Key, Value);

			Assert.AreEqual(int.Parse(properties[Key], CultureInfo.InvariantCulture), Value);
		}
	}
}