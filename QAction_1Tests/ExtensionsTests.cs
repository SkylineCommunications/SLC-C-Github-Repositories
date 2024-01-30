namespace Skyline.Protocol.Extensions.Tests
{
	using System.Collections.Generic;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Newtonsoft.Json;

	using Skyline.Protocol.JSON.Converters;
	using Skyline.Protocol.Tables.WorkflowsTable.Requests;

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
		public void WorkflowJsonConverter()
		{
			var request1 = new AddAutomationScriptCIWorkflowRequest("Arne/Repo1", "Arne_Repo1", "Somekey1");
			var request2 = new AddAutomationScriptCDWorkflowRequest("Arne/Repo2", "Arne_Repo2", "Somekey2");

			var input = JsonConvert.SerializeObject(new IWorkflowsTableRequest[] { request1, request2 });

			var result = JsonConvert.DeserializeObject<IWorkflowsTableRequest[]>(input, new WorkflowTableRequestConverter());

			Assert.IsTrue(result[0] is AddAutomationScriptCIWorkflowRequest);
			Assert.IsTrue(result[1] is AddAutomationScriptCDWorkflowRequest);
		}
	}
}