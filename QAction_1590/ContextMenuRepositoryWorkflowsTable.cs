// Ignore Spelling: Workflows

namespace QAction_1590
{
	using System;

	using Newtonsoft.Json;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows.Data;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.DataMiner.Utils.Table.ContextMenu;

	using Extensions = Skyline.Protocol.Extensions.Extensions;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
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
			Message request = default;
			switch ((WorkflowType)Convert.ToInt32(Data[1]))
			{
				case WorkflowType.AutomationScriptCI:
					request = new AddAutomationScriptCIWorkflowRequest
					{
						RepositoryId = new RepositoryId(owner, name),
						Data = new AutomationScriptCIWorkflowData
						{
							SonarCloudProjectID = String.Empty,
							DataMinerKey = String.Empty,
						},
					};
					break;

				case WorkflowType.AutomationScriptCICD:
					request = new AddAutomationScriptCICDWorkflowRequest
					{
						RepositoryId = new RepositoryId(owner, name),
						Data = new AutomationScriptCICDWorkflowData
						{
							SonarCloudProjectID = String.Empty,
							DataMinerKey = String.Empty,
						},
					};
					break;

				case WorkflowType.ConnectorCI:
					request = new AddConnectorCIWorkflowRequest
					{
						RepositoryId = new RepositoryId(owner, name),
						Data = new ConnectorCIWorkflowData
						{
							SonarCloudProjectID = String.Empty,
							DataMinerKey = String.Empty,
						},
					};
					break;

				case WorkflowType.NugetSolutionCICD:
					request = new AddNugetCICDWorkflowRequest
					{
						RepositoryId = new RepositoryId(owner, name),
						Data = new NugetCICDWorkflowData
						{
							SonarCloudProjectID = String.Empty,
							NugetApiKey = String.Empty,
						},
					};
					break;

				default:
					Protocol.Log($"QA{Protocol.QActionID}|AddWorkflow|This type of workflow is not supported yet.", LogType.Information, LogLevel.NoLogging);
					break;
			}

			if (request != default)
			{
				request.TryExecute(Protocol, Protocol, Mapping.MessageToExecutorMapping, out _);
			}
		}
	}
}
