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
		void SendMessage(string message, params object[] parameters);
	}
}