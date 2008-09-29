namespace NAntExtensions.TeamCity.Types
{
	/// <summary>
	/// The build status to report to TeamCity.
	/// </summary>
	public enum StatusType
	{
		/// <summary>
		/// Normal build status. We suspect this means that the current build status is not changed.
		/// </summary>
		Normal,
		/// <summary>
		/// The build failed.
		/// </summary>
		Error,
		/// <summary>
		/// The build failed.
		/// </summary>
		Failure,
		/// <summary>
		/// The build is successful.
		/// </summary>
		Success
	}
}