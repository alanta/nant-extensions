using System;

namespace NAntExtensions.TeamCity.Common.Messaging
{
	public interface ITeamCityMessageProvider
	{
		void TestSuiteStarted(string assemblyName);
		void TestSuiteFinished(string assemblyName);
		void TestStarted(string testName);
		void TestIgnored(string testName, string message);
		void TestFailed(string testName, Exception exception);
		void TestFailed(string testName, ExceptionInfo exceptionInfo);
		void TestOutputStream(string testName, string outputStream);
		void TestErrorStream(string testName, string errorStream);
		void TestFinished(string testName);
		void TestFinished(string testName, TimeSpan duration);

		void BuildNumber(string newBuildNumber);
		void BuildStatus(string newBuildNumber);
		void PublishBuildArtifacts(string pathToArtifacts);
		
		void ProgressStart(string message);
		void ProgressMessage(string message);
		void ProgressFinished(string message);
		
		void ImportData(string type, string pathToDataFile);

		void SendMessage(string message, params object[] parameters);
	}
}