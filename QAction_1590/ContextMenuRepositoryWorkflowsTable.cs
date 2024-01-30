// Ignore Spelling: Workflows

namespace QAction_1590
{
	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Table.ContextMenu;
	using Skyline.Protocol.Tables.WorkflowsTable.Requests;

	using Extensions = Skyline.Protocol.Extensions.Extensions;

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
					Protocol.SetParameter(Parameter.repositoryworkflow_changerequest, JsonConvert.SerializeObject(new[] { new BaseWorkflowRequest(Data[0], Extensions.ParseEnumDescription<WorkflowType>(Data[1]), WorkflowAction.Add) }));
					break;

				default:
					Protocol.Log("QA" + Protocol.QActionID + "|ContextMenuRepositoryWorkflowsTable|Process|Unexpected ContextMenu value '" + ActionRaw + "'", LogType.Error, LogLevel.NoLogging);
					break;
			}
		}
	}
}
