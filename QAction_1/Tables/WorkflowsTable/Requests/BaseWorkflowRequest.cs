// Ignore Spelling: Workflow Workflows

namespace Skyline.Protocol.Tables.WorkflowsTable.Requests
{
	using System;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.API.Workflows;

	[Serializable]
	public class BaseWorkflowRequest : IWorkflowsTableRequest
	{
		public BaseWorkflowRequest(string repositoryId, WorkflowType type, WorkflowAction action)
		{
			RepositoryId = repositoryId;
			WorkflowType = type;
			Action = action;
		}

		public string RepositoryId { get; set; }

		public WorkflowAction Action { get; set; }

		public WorkflowType WorkflowType { get; set; }

		public virtual Workflow CreateWorkflow(SLProtocol protocol)
		{
			return WorkflowFactory.Create(WorkflowType);
		}
	}
}
