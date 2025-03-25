// Ignore Spelling: Github API

namespace Skyline.Protocol.API
{
	using Newtonsoft.Json;

	public class GithubError
	{
		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("documentation_url")]
#pragma warning disable S3996 // URI properties should not be strings
		public string DocumentationUrl { get; set; }
#pragma warning restore S3996 // URI properties should not be strings

		[JsonProperty("status")]
		public string Status { get; set; }
	}
}
