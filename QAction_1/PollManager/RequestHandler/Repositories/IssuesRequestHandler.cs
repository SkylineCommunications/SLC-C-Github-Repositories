namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesIssuesRequest(SLProtocol protocol)
		{
			HandleRepositoriesIssuesRequest(protocol, PollingConstants.PerPage, 1);
		}

		public static void HandleRepositoriesIssuesRequest(SLProtocol protocol, int perPage, int page)
		{
			var table = RepositoriesTable.GetTable(protocol);
			foreach (var row in table.Rows)
			{
				HandleRepositoriesIssuesRequest(protocol, row.Owner, row.Name, perPage, page);
			}
		}

		public static void HandleRepositoriesIssuesRequest(SLProtocol protocol, string owner, string name, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getrepositoryissuesurl, $"repos/{owner}/{name}/issues?per_page={perPage}&page={page}&state=all");
			protocol.CheckTrigger(202);
		}
	}
}
