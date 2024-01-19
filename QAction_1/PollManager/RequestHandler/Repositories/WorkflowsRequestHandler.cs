// Ignore Spelling: Workflows

namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesWorkflowsRequest(SLProtocol protocol)
		{
			HandleRepositoriesWorkflowsRequest(protocol, PollingConstants.PerPage, 1);
		}

		public static void HandleRepositoriesWorkflowsRequest(SLProtocol protocol, int perPage, int page)
		{
			var table = RepositoriesTable.GetTable(protocol);
			foreach (var row in table.Rows)
			{
				HandleRepositoriesWorkflowsRequest(protocol, row.Owner, row.Name, perPage, page);
			}
		}

		public static void HandleRepositoriesWorkflowsRequest(SLProtocol protocol, string owner, string name, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getrepositoryworkflowsurl, $"repos/{owner}/{name}/actions/workflows?per_page={perPage}&page={page}");
			protocol.CheckTrigger(205);
		}
	}
}
