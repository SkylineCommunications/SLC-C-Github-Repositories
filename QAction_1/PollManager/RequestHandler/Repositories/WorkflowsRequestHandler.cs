// Ignore Spelling: Workflows Workflow

namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;
	using Skyline.Protocol.API.Workflows;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.Tables;
	using Skyline.Protocol.Tables.WorkflowsTable.Requests;
	using Skyline.Protocol.YAML;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesWorkflowsRequest(SLProtocol protocol)
		{
			HandleRepositoriesWorkflowsRequest(protocol, PollingConstants.PerPage, 1);
		}

		public static void HandleRepositoriesWorkflowsRequest(SLProtocol protocol, int perPage, int page)
		{
			var table = RepositoriesTable.GetTable(protocol);
			var first = table.Rows[0];
			protocol.SetParameter(Parameter.getrepositoryworkflowsqueue, JsonConvert.SerializeObject(table.Rows.Select(x => x.FullName).Skip(1)));
			HandleRepositoriesWorkflowsRequest(protocol, first.Owner, first.Name, perPage, page);
		}

		public static void HandleRepositoriesWorkflowsRequest(SLProtocol protocol, string owner, string name, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getrepositoryworkflowsurl, $"repos/{owner}/{name}/actions/workflows?per_page={perPage}&page={page}");
			protocol.CheckTrigger(205);
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
			RepositoriesRequestHandler.CreateRepositoryContent(
				protocol,
				repositoryId,
				$".github/workflows/{workflow.Name}.yml",
				YamlConvert.SerializeObject(workflow),
				$"Adding a new workflow: {workflow.Name}");
		}
	}
}
