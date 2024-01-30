// Ignore Spelling: API

namespace Skyline.Protocol.API.Secrets
{
	using System;

	using Newtonsoft.Json;

	[Serializable]
	public class PutRepositorySecret
	{
		[JsonProperty("encrypted_value")]
		public string EncryptedValue { get; set; }

		[JsonProperty("key_id")]
		public string KeyID { get; set; }
	}
}
