// Ignore Spelling: API

namespace Skyline.Protocol.API.Variables
{
	using System;

	using Newtonsoft.Json;

	[Serializable]
	public class VariablePair
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("value")]
		public string Value { get; set; }
	}
}
