using System;

using Newtonsoft.Json;

using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows.Data;
using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol.InterApp;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocol protocol)
	{
		try
		{
			var trigger = protocol.GetTriggerParameter();
			switch (trigger)
			{
				case Parameter.Write.addworkflowbutton_550:
					AddWorkflow(protocol);
					break;

				case Parameter.Write.addworkflowrepository_651:
					// TODO when I implement a branches table.
					break;

				default:
					protocol.Log($"QA{protocol.QActionID}|Run|Trigger not implemented yet.", LogType.Error, LogLevel.NoLogging);
					break;
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static void AddWorkflow(SLProtocol protocol)
	{
		var parameters = Array.ConvertAll(
			(object[])protocol.GetParameters(new uint[]
			{
				Parameter.addworkflowrepository,
				Parameter.addworkflowbranch,
				Parameter.addworkflowworkflow,
			}), Convert.ToString);

		var owner = parameters[0].Split('/')[0];
		var name = parameters[0].Split('/')[1];
		Message request = default;
		switch ((WorkflowType)Convert.ToInt32(parameters[2]))
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
				protocol.Log($"QA{protocol.QActionID}|AddWorkflow|This type of workflow is not supported yet.", LogType.Information, LogLevel.NoLogging);
				break;
		}

		if(request != default)
		{
			request.TryExecute(protocol, protocol, Mapping.MessageToExecutorMapping, out _);
		}
	}
}
