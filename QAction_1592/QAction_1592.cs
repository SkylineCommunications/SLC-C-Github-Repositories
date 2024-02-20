// Ignore Spelling: Workflow

using System;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Github.Repositories.Core.Workflows;
using Skyline.Protocol.API.Workflows;
using Skyline.Protocol.JSON.Converters;
using Skyline.Protocol.PollManager.RequestHandler.Repositories;

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
		// Create workflow file
		var workflow = WorkflowFactory.Create(request);

		// Create the required secrets
		var secrets = request.GetType().GetProperties()
			.Where(prop => prop.GetCustomAttribute<WorkflowSecretAttribute>() != null);
		foreach(var secret in secrets)
		{
			var attr = secret.GetCustomAttribute<WorkflowSecretAttribute>();
			RepositoriesRequestHandler.CreateRepositorySecret(protocol, request.RepositoryId, attr.Name, Convert.ToString(secret.GetValue(request)));
		}

		// Do the actual commit to the repository
		RepositoriesRequestHandler.CreateRepositoryWorkflow(protocol, request.RepositoryId, workflow);
	}
}