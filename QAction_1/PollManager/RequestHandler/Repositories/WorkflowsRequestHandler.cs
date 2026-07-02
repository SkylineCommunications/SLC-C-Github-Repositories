// Ignore Spelling: Workflows Workflow

namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;

	using Newtonsoft.Json;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;
	using Skyline.Protocol.API.Workflows;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.Tables;
	using Skyline.Protocol.YAML;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesWorkflowsRequest(SLProtocol protocol, bool executeNow)
		{
			var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Repositories_Workflows)?.PageLimit ?? PollingConstants.PerPage;
			HandleRepositoriesWorkflowsRequest(protocol, perPage, 1, executeNow);
		}

		public static void HandleRepositoriesWorkflowsRequest(SLProtocol protocol, int perPage, int page, bool executeNow)
		{
			var rows = SLTables.Repositories.GetPrimaryKeys(protocol);
			if (!rows.Any())
			{
				return;
			}

			protocol.SetParameter(Parameter.getrepositoryworkflowsqueue, JsonConvert.SerializeObject(rows.Skip(1)));
			HandleRepositoriesWorkflowsRequest(protocol, rows[0], perPage, page, executeNow);
		}

		public static void HandleRepositoriesWorkflowsRequest(SLProtocol protocol, string repositoryId, int perPage, int page, bool executeNow)
		{
			protocol.SetParameter(Parameter.getrepositoryworkflowsurl, $"repos/{repositoryId}/actions/workflows?per_page={perPage}&page={page}");
			var trigger = executeNow ? Triggers.GetRepositoryWorkflowsNow : Triggers.GetRepositoryWorkflows;
			protocol.CheckTrigger((int)trigger);
		}

		public static void CreateRepositoryWorkflow(SLProtocol protocol, string repositoryId, WorkflowType type)
		{
			RepositoriesRequestHandler.CreateRepositoryContent(
				protocol,
				repositoryId,
				$".github/workflows/{type.FriendlyDescription()}.yml",
				YamlConvert.SerializeObject(WorkflowFactory.Create(type)),
				$"Adding a new workflow: {type.FriendlyDescription()}");
		}

		public static void CreateRepositoryWorkflow(SLProtocol protocol, string repositoryId, Workflow workflow)
		{
			CreateRepositoryWorkflow(protocol, repositoryId, workflow.Name, YamlConvert.SerializeObject(workflow));
		}

		public static void CreateRepositoryWorkflow(SLProtocol protocol, string repositoryId, string workflowName, string workflowContent)
		{
			RepositoriesRequestHandler.CreateRepositoryContent(
				protocol,
				repositoryId,
				$".github/workflows/{workflowName}.yml",
				workflowContent,
				$"Adding a new workflow: {workflowName}");
		}

		public static void ExecuteWorkflow(SLProtocol protocol, string repositoryId, string reference, string workflowIdOrName, Dictionary<string, string> inputs)
		{
			var body = new WorkflowExecutionRequest
			{
				Reference = reference,
				Inputs = inputs ?? new Dictionary<string, string>(),
			};

			var sets = new Dictionary<int, object>
			{
				{ Parameter.postworkflowexecutionurl_131, $"repos/{repositoryId}/actions/workflows/{HttpUtility.UrlEncode(workflowIdOrName)}/dispatches" },
				{ Parameter.postworkflowexecutionbody_181, JsonConvert.SerializeObject(body) },
			};

			protocol.SetParameters(sets.Keys.ToArray(), sets.Values.ToArray());
			protocol.CheckTrigger((int)Triggers.PostWorkflowExecutionNow);
		}
	}
}
