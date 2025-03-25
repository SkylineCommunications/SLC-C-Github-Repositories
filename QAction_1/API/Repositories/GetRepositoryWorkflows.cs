// Ignore Spelling: Utils Github API Workflows

namespace Skyline.DataMiner.Utils.Github.API.V20221128.Repositories
{
	using System;
	using System.Collections.Generic;

	using Newtonsoft.Json;

	// Root myDeserializedClass = SecureNewtonsoftDeserialization.DeserializeObject<RepositoryWorkflowsResponse>(myJsonResponse);
	public class RepositoryWorkflowsResponse
	{
		[JsonProperty("total_count")]
		public int TotalCount { get; set; }

		[JsonProperty("workflows")]
		public List<Workflow> Workflows { get; set; }
	}

	public class Workflow
	{
		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("node_id")]
		public string NodeId { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("path")]
		public string Path { get; set; }

		[JsonProperty("state")]
		public string State { get; set; }

		[JsonProperty("created_at")]
		public DateTime CreatedAt { get; set; }

		[JsonProperty("updated_at")]
		public DateTime UpdatedAt { get; set; }

		[JsonProperty("deleted_at")]
		public DateTime DeletedAt { get; set; }

		[JsonProperty("url")]
#pragma warning disable S3996 // URI properties should not be strings
		public string Url { get; set; }
#pragma warning restore S3996 // URI properties should not be strings

		[JsonProperty("html_url")]
#pragma warning disable S3996 // URI properties should not be strings
		public string HtmlUrl { get; set; }
#pragma warning restore S3996 // URI properties should not be strings

		[JsonProperty("badge_url")]
#pragma warning disable S3996 // URI properties should not be strings
		public string BadgeUrl { get; set; }
#pragma warning restore S3996 // URI properties should not be strings
	}
}
