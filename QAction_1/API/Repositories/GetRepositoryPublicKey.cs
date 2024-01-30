// Ignore Spelling: API

namespace Skyline.Protocol.API.Repositories
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
