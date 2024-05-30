// Ignore Spelling: Workflows

namespace QAction_1590
{
	using System;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows.Data;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Table.ContextMenu;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.InterApp;

	internal enum Action
	{
		Add = 1,
	}

	internal class ContextMenuRepositoryWorkflowsTable : ContextMenu<Action>
	{
		public ContextMenuRepositoryWorkflowsTable(SLProtocol protocol, object contextMenuData, int tablePid)
			: base(protocol, contextMenuData, tablePid)
		{
		}

		public override void ProcessContextMenuAction()
		{
			switch (Action)
			{
				case Action.Add:
					Add();
					break;

				default:
					Protocol.Log("QA" + Protocol.QActionID + "|ContextMenuRepositoryWorkflowsTable|Process|Unexpected ContextMenu value '" + ActionRaw + "'", LogType.Error, LogLevel.NoLogging);
					break;
			}
		}

		protected void Add()
		{
			var owner = Data[0].Split('/')[0];
			var name = Data[0].Split('/')[1];
			var workflowType = (WorkflowType)Convert.ToInt32(Data[1]);
			Message request = WorkflowMapping.CreateEmptyWorkflowMessage(workflowType, owner, name);

			if (request != default)
			{
				request.TryExecute(Protocol, Protocol, Mapping.MessageToExecutorMapping, out _);
			}
		}
	}
}
