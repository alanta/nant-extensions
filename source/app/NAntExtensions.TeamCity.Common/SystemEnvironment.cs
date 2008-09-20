using System;

namespace NAntExtensions.TeamCity.Common
{
	public class SystemEnvironment:IEnvironment
	{
		public string GetEnvironmentVariable(string variable)
		{
			return Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Process);
		}
	}
}