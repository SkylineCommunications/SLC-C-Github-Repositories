// Ignore Spelling: Workflow Workflows

namespace Skyline.Protocol.Tables
{
	using System;

	using Skyline.DataMiner.Scripting;

	public delegate void RepositoryWorkflowsTableEventHandler(object sender, RepositoryWorkflowsEventArgs e);

	public enum WorkflowChange
	{
		Add,
		Remove,
	}

	public class RepositoryWorkflowsEventArgs : EventArgs
	{
		public RepositoryWorkflowsEventArgs(SLProtocol protocol, WorkflowChange changeType, string repositoryId, params string[] workflows)
		{
			Protocol = protocol;
			Type = changeType;
			RepositoryId = repositoryId;
			Workflows = workflows;
		}

		public SLProtocol Protocol { get; }

		public WorkflowChange Type { get; }

		public string RepositoryId { get; }

		public string[] Workflows { get; }
	}
}
