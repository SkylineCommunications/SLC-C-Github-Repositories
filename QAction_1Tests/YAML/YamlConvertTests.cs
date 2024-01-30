using Microsoft.VisualStudio.TestTools.UnitTesting;

using Skyline.Protocol.YAML;

namespace Skyline.Protocol.YAML.Tests
{
	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Skyline.Protocol.API.Workflows;
	using Skyline.Protocol.YAML;

	[TestClass]
	public class YamlConvertTests
	{
		[TestMethod]
		public void SerializeObjectTest()
		{
			// Arrange
			var flow = WorkflowFactory.CreateCIWorkflow("SkylineCommunications_RBMU-AS-MCR-Swimlanes");

			// Execute
			var result = YamlConvert.SerializeObject(flow);

			// Assert
			Assert.IsNotNull(result);
		}

		[TestMethod()]
		public void DeserializeObjectTest()
		{
			// Arrange
			var flow = WorkflowFactory.CreateCIWorkflow("SkylineCommunications_RBMU-AS-MCR-Swimlanes");

			// Execute
			var result = YamlConvert.SerializeObject(flow);
			var workflow = YamlConvert.DeserializeObject<Workflow>(result);

			// Assert
			Assert.IsNotNull(result);
		}
	}
}