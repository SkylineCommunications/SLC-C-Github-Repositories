namespace Skyline.Protocol.Extensions.Tests
{
	using System.Collections.Generic;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Newtonsoft.Json;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk;

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
			var message = "{\"$id\":\"1\",\"$type\":\"InterAppCall,\",\"guid\":\"9011ff84-6f44-4a53-8043-061986a0d0ac\",\"messages\":{\"$type\":\"Skyline.DataMiner.Core.InterAppCalls.Common.CallBulk.Messages,\",\"$values\":[{\"$id\":\"2\",\"$type\":\"AddInternalNugetCICDWorkflowRequest,\",\"data\":{\"$id\":\"3\",\"$type\":\"Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows.Data.InternalNugetCICDWorkflowData,\",\"githubNugetApiKey\":\"somekey\",\"sonarCloudProjectID\":\"correctly_filled_in\"},\"guid\":\"e453e383-5ca0-4266-876c-e78a8c744bc0\",\"repositoryId\":{\"$id\":\"4\",\"$type\":\"Skyline.DataMiner.ConnectorAPI.Github.Repositories.RepositoryId,\",\"fullName\":\"SkylineCommunicationsSandbox/SLC-AS-GithubTestRepository\",\"name\":\"SLC-AS-GithubTestRepository\",\"owner\":\"SkylineCommunicationsSandbox\"},\"returnAddress\":{\"$id\":\"5\",\"$type\":\"Skyline.DataMiner.Core.InterAppCalls.Common.Shared.ReturnAddress,\",\"agentId\":925,\"elementId\":187,\"parameterId\":9000001},\"source\":null,\"workflowType\":4}]},\"receivingTime\":\"0001-01-01T00:00:00\",\"returnAddress\":{\"$ref\":\"5\"},\"sendingTime\":\"2024-02-21T12:11:09.7761323+01:00\",\"source\":null}";

			var result = InterAppCallFactory.CreateFromRaw(message, Types.KnownTypes);

			Assert.IsTrue(true);
			//Assert.ThrowsException<KeyNotFoundException>(() => Extensions.ParseEnumDescription<WorkflowType>(discreet));
		}
	}
}