// Ignore Spelling: API Utils Github

namespace Skyline.DataMiner.Utils.Github.API.V20221128.Repositories
{
	using System;
	using System.Collections.Generic;

	using Newtonsoft.Json;

	// var myDeserializedClass = SecureNewtonsoftDeserialization.DeserializeObject<RepositoryTopics>(myJsonResponse);

	[Serializable]
	public class RepositoryTopics
	{
		[JsonProperty("names")]
		public List<string> Names { get; set; }
	}
}
