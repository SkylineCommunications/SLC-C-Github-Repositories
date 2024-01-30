// Ignore Spelling: Workflow Workflows dataminer DataMiner

namespace Skyline.Protocol.Tables.WorkflowsTable.Requests
{
	using System;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.API.Workflows;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;

	[Serializable]
	public class AddAutomationScriptCDWorkflowRequest : BaseWorkflowRequest
	{
		public AddAutomationScriptCDWorkflowRequest(string repositoryId, string sonarCloudProjectId, string dataminerKey)
			: base(repositoryId, WorkflowType.AutomationScriptCD, WorkflowAction.Add)
		{
			SonarCloudProjectID = sonarCloudProjectId;
			DataMinerKey = dataminerKey;
		}

		public string SonarCloudProjectID { get; private set; }

		public string DataMinerKey { get; private set; }

		public override Workflow CreateWorkflow(SLProtocol protocol)
		{
			// Create workflow file
			var workflow = WorkflowFactory.CreateCDWorkflow(SonarCloudProjectID);

			// Create the required secrets
			RepositoriesRequestHandler.CreateRepositorySecret(protocol, RepositoryId, "DATAMINER_DEPLOY_KEY", DataMinerKey);

			return workflow;
		}
	}
}
