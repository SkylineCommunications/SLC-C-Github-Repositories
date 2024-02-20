// Ignore Spelling: Workflow JSON

namespace Skyline.Protocol.JSON.Converters
{
	using System;
	using System.Collections.Generic;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Skyline.DataMiner.Utils.Github.Repositories.Core.Workflows;

	public class WorkflowTableRequestConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return typeof(IEnumerable<IWorkflowsTableRequest>).IsAssignableFrom(objectType);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var result = new List<IWorkflowsTableRequest>();
			var items = JArray.Load(reader);
			foreach (var item in items)
			{
				var baseRequest = item.ToObject<BaseWorkflowRequest>();
				switch(baseRequest.WorkflowType)
				{
					case WorkflowType.AutomationScriptCI:
						result.Add(item.ToObject<AddAutomationScriptCIWorkflowRequest>());
						continue;

					case WorkflowType.AutomationScriptCICD:
						result.Add(item.ToObject<AddAutomationScriptCICDWorkflowRequest>());
						continue;

					case WorkflowType.ConnectorCI:
						result.Add(item.ToObject<AddConnectorCIWorkflowRequest>());
						continue;

					case WorkflowType.NugetSolutionCICD:
						result.Add(item.ToObject<AddNugetCICDWorkflowRequest>());
						continue;

					default:
						continue;
				}
			}

			return result.ToArray();
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();

		public override bool CanRead => true;

		public override bool CanWrite => false;
	}
}
