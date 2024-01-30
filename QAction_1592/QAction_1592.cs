// Ignore Spelling: Workflow

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Newtonsoft.Json;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol.API.Workflows;
using Skyline.Protocol.JSON.Converters;
using Skyline.Protocol.PollManager;
using Skyline.Protocol.PollManager.RequestHandler.Repositories;
using Skyline.Protocol.Tables;
using Skyline.Protocol.Tables.WorkflowsTable.Requests;

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
			var param = Convert.ToString(protocol.GetParameter(Parameter.repositoryworkflow_changerequest));
			var requests = JsonConvert.DeserializeObject<IWorkflowsTableRequest[]>(param, new WorkflowTableRequestConverter());
			foreach (var request in requests)
			{
				switch (request.Action)
				{
					case WorkflowAction.Add:
						Add(protocol, request);
						break;

					default:
						throw new NotSupportedException("The give action is not supported yet.");
				}
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static void Add(SLProtocol protocol, IWorkflowsTableRequest request)
	{
		var workflow = request.CreateWorkflow(protocol);
		RepositoriesRequestHandler.CreateRepositoryWorkflow(protocol, request.RepositoryId, workflow);
	}
}