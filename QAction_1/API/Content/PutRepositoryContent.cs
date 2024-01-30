// Ignore Spelling: Committer API Sha

namespace Skyline.Protocol.API.Content
{
	using System;

	using Newtonsoft.Json;

	[Serializable]
	public class PutRepositoryContent
	{
		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("content")]
		public string Content { get; set; }

		[JsonProperty("branch", NullValueHandling = NullValueHandling.Ignore)]
		public string Branch { get; set; }

		[JsonProperty("sha", NullValueHandling = NullValueHandling.Ignore)]
		public string Sha { get; set; }

		[JsonProperty("committer", NullValueHandling = NullValueHandling.Ignore)]
		public Committer Committer { get; set; }
	}

	[Serializable]
	public class Committer
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }
	}
}
