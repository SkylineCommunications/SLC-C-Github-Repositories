using System;

using Newtonsoft.Json;

using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Github.Repositories.Core.Workflows;

using Extensions = Skyline.Protocol.Extensions.Extensions;

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

		// IWorkflowsTableRequest request = new BaseWorkflowRequest(parameters[0], (WorkflowType)Convert.ToInt32(parameters[2]), WorkflowAction.Add);
		IWorkflowsTableRequest request = default(BaseWorkflowRequest);
		switch ((WorkflowType)Convert.ToInt32(parameters[2]))
		{
			case WorkflowType.AutomationScriptCI:
				request = new AddAutomationScriptCIWorkflowRequest(parameters[0], String.Empty, String.Empty);
				break;

			case WorkflowType.AutomationScriptCICD:
				request = new AddAutomationScriptCICDWorkflowRequest(parameters[0], String.Empty, String.Empty);
				break;

			case WorkflowType.ConnectorCI:
				request = new AddConnectorCIWorkflowRequest(parameters[0], String.Empty, String.Empty);
				break;

			case WorkflowType.NugetSolutionCICD:
				request = new AddNugetCICDWorkflowRequest(parameters[0], String.Empty, String.Empty);
				break;

			default:
				protocol.Log($"QA{protocol.QActionID}|AddWorkflow|This type of workflow is not supported yet.", LogType.Information, LogLevel.NoLogging);
				break;
		}

		if(request != default(BaseWorkflowRequest))
		{
			protocol.SetParameter(Parameter.repositoryworkflow_changerequest, JsonConvert.SerializeObject(new[] { request }));
		}
	}
}
