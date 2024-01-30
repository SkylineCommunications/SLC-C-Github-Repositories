// Ignore Spelling: Workflow Workflows

namespace Skyline.Protocol.Tables.WorkflowsTable.Requests
{
	using System.ComponentModel;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.API.Workflows;

	public enum WorkflowType
	{
		[Description("Automation Script CI")]
		AutomationScriptCI = 0,

		[Description("Automation Script CD")]
		AutomationScriptCD = 1,
	}

	public enum WorkflowAction
	{
		Add = 0,
	}

	public interface IWorkflowsTableRequest
	{
		string RepositoryId { get; }

		WorkflowAction Action { get; }

		WorkflowType WorkflowType { get; }

		Workflow CreateWorkflow(SLProtocol protocol);
	}
}
