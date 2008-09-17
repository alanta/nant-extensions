using NAnt.Core;

namespace NAntExtensions.TeamCity.Common
{
	public static class PropertyDictionaryHelper
	{
		public static void AddOrUpdateInt(PropertyDictionary instance, string key, int value)
		{
			int parsedValue;
			if (!int.TryParse(instance[key], out parsedValue))
			{
				parsedValue = 0;
			}

			parsedValue += value;
			instance[key] = parsedValue.ToString();
		}
	}
}