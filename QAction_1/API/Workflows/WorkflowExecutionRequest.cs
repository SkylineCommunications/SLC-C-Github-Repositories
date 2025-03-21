// Ignore Spelling: API Workflows

namespace Skyline.Protocol.API.Workflows
{
	using System;
	using System.Collections.Generic;

	using Newtonsoft.Json;

	[Serializable]
	public class WorkflowExecutionRequest
	{
		[JsonProperty("ref")]
		public string Reference { get; set; }

		[JsonProperty("inputs")]
		public Dictionary<string, object> Inputs { get; set; }
	}
}
