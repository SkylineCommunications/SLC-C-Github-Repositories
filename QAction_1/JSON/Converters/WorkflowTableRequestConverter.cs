// Ignore Spelling: Workflow JSON

namespace Skyline.Protocol.JSON.Converters
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Newtonsoft.Json;
	using Newtonsoft.Json.Linq;

	using Skyline.Protocol.Tables.WorkflowsTable.Requests;

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

					case WorkflowType.AutomationScriptCD:
						result.Add(item.ToObject<AddAutomationScriptCDWorkflowRequest>());
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
