namespace Skyline.Protocol.API
{
	using System.Text.RegularExpressions;

	/// <summary>
	/// Helper methods for parsing GitHub API URLs.
	/// </summary>
	public static class GithubUrlHelper
	{
		private static readonly Regex RepoOwnerNameRegex = new Regex(
			@"repos/(?<owner>[^/]+)/(?<name>[^/]+)",
			RegexOptions.Compiled);

		/// <summary>
		/// Extracts the repository owner and name from a GitHub API URL containing a <c>repos/{owner}/{name}</c> segment.
		/// </summary>
		/// <param name="url">The URL to parse.</param>
		/// <param name="owner">When successful, contains the repository owner.</param>
		/// <param name="name">When successful, contains the repository name.</param>
		/// <returns><c>true</c> if the URL contains a valid <c>repos/{owner}/{name}</c> segment; otherwise <c>false</c>.</returns>
		public static bool TryParseRepoOwnerAndName(string url, out string owner, out string name)
		{
			var match = RepoOwnerNameRegex.Match(url);
			if (match.Success)
			{
				owner = match.Groups["owner"].Value;
				name = match.Groups["name"].Value;
				return true;
			}

			owner = null;
			name = null;
			return false;
		}
	}
}
