namespace Skyline.Protocol.Extensions.Tests
{
	using System.Collections.Generic;
	using System.IO;
	using System.Web;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Newtonsoft.Json;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	[TestClass]
	public class ExtensionsTests
	{
		[TestMethod]
		public void ParseEnumDescriptionTest()
		{
			var discreet = "Automation Script CI";

			var result = Extensions.ParseEnumDescription<WorkflowType>(discreet);

			Assert.AreEqual<WorkflowType>(WorkflowType.AutomationScriptCI, result);
		}

		[TestMethod]
		public void ParseEnumDescriptionFailTest()
		{
			var discreet = "NOT EXISTING";

			Assert.ThrowsException<KeyNotFoundException>(() => Extensions.ParseEnumDescription<WorkflowType>(discreet));
		}

		[TestMethod]
		public void DeserializeInterApp()
		{
			var message = "{\"$id\":\"1\",\"$type\":\"InterAppCall, \",\"expectsReply\":true,\"guid\":\"bc53dc35-4918-43be-b49a-e3fecf043ec3\",\"messages\":{\"$type\":\"Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk.Messages, \",\"$values\":[{\"$id\":\"2\",\"$type\":\"Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.GenericInterAppMessage`1[Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories.CreateRepositoryRequest], \",\"brokerReturnAddress\":\"InterApp.PID63844.GUID9896c5ee.337d61de0be843b194b676e59645e3d5\",\"data\":{\"$id\":\"3\",\"$type\":\"Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories.CreateRepositoryRequest, \",\"data\":{\"$id\":\"4\",\"$type\":\"Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories.Data.CreateRepositoryData, \",\"description\":\"Contains a GQI Data Source that fetches the DOM History of a DOM Instance.\",\"name\":\"SLC-GQIDS-DOM-History\",\"organizationId\":\"SkylineCommunications\",\"public\":false}},\"expectsReply\":true,\"guid\":\"f562dedf-7651-4f1d-85ef-5d296f2c1a34\",\"returnAddress\":{\"$id\":\"5\",\"$type\":\"Skyline.DataMiner.Core.InterAppCalls.Common.Shared.ReturnAddress, \",\"agentId\":925,\"elementId\":187,\"parameterId\":9000001},\"source\":null}]},\"receivingTime\":\"0001-01-01T00:00:00\",\"returnAddress\":{\"$ref\":\"5\"},\"sendingTime\":\"2024-05-29T15:22:47.8139151+02:00\",\"source\":null}";

			var result = InterAppCallFactory.CreateFromRaw(message, Types.KnownTypes);

			Assert.IsTrue(true);
			//Assert.ThrowsException<KeyNotFoundException>(() => Extensions.ParseEnumDescription<WorkflowType>(discreet));
		}

		[TestMethod]
		public void WorkflowNameParsing()
		{
			var name = "DataMiner CI Automation";
			var file = "DataMiner+CI+Automation.yml";

			var fileName = Path.GetFileNameWithoutExtension(file);
			var iacFileName = HttpUtility.UrlEncode(name);

			Assert.IsTrue(fileName == iacFileName);
		}
	}
}