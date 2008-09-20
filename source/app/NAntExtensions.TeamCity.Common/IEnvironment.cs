namespace NAntExtensions.TeamCity.Common
{
	public interface IEnvironment
	{
		string GetEnvironmentVariable(string variable);
	}
}