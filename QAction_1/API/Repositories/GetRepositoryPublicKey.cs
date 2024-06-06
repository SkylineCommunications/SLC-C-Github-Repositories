// Ignore Spelling: API Utils Github

namespace Skyline.DataMiner.Utils.Github.API.V20221128.Repositories
{
	using System;

	using Newtonsoft.Json;

	[Serializable]
	public class PublicKey
	{
		[JsonProperty("key_id")]
		public string KeyID { get; set; }

		[JsonProperty("key")]
		public string Key { get; set; }
	}
}
