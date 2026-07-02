// Ignore Spelling: Utils Github API

namespace Skyline.DataMiner.Utils.Github.API.V20221128.Organizations
{
	using System;

	using Newtonsoft.Json;

	// Root myDeserializedClass = SecureNewtonsoftDeserialization.DeserializeObject<List<Team>>(myJsonResponse);

	public class Parent
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("node_id")]
		public string NodeId { get; set; }

		[JsonProperty("slug")]
		public string Slug { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("privacy")]
		public string Privacy { get; set; }

		[JsonProperty("notification_setting")]
		public string NotificationSetting { get; set; }

		[JsonProperty("url")]
		public Uri Url { get; set; }

		[JsonProperty("html_url")]
		public Uri HtmlUrl { get; set; }

		[JsonProperty("members_url")]
		public string MembersUrl { get; set; }

		[JsonProperty("repositories_url")]
		public Uri RepositoriesUrl { get; set; }

		[JsonProperty("permission")]
		public string Permission { get; set; }
	}

	public class Team
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("id")]
		public long Id { get; set; }

		[JsonProperty("node_id")]
		public string NodeId { get; set; }

		[JsonProperty("slug")]
		public string Slug { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("privacy")]
		public string Privacy { get; set; }

		[JsonProperty("notification_setting")]
		public string NotificationSetting { get; set; }

		[JsonProperty("url")]
		public Uri Url { get; set; }

		[JsonProperty("html_url")]
		public Uri HtmlUrl { get; set; }

		[JsonProperty("members_url")]
		public string MembersUrl { get; set; }

		[JsonProperty("repositories_url")]
		public Uri RepositoriesUrl { get; set; }

		[JsonProperty("permission")]
		public string Permission { get; set; }

		[JsonProperty("parent")]
		public Parent Parent { get; set; }
	}
}
