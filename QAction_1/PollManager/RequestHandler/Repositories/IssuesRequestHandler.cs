namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesIssuesRequest(SLProtocol protocol, bool executeNow)
		{
			HandleRepositoriesIssuesRequest(protocol, 1, executeNow);
		}

		public static void HandleRepositoriesIssuesRequest(SLProtocol protocol, int page, bool executeNow)
		{
			var repositories = SLTables.Repositories.GetPrimaryKeys(protocol);
			foreach (var row in repositories)
			{
				HandleRepositoriesIssuesRequest(protocol, row, page, executeNow);
			}
		}

		public static void HandleRepositoriesIssuesRequest(SLProtocol protocol, string repositoryId, int page, bool executeNow)
		{
			var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Repository_Issues)?.PageLimit ?? PollingConstants.PerPage;
			protocol.SetParameter(Parameter.getrepositoryissuesurl, $"repos/{repositoryId}/issues?per_page={perPage}&page={page}&state=all");
			var trigger = executeNow ? Triggers.GetRepositoryIssuesNow : Triggers.GetRepositoryIssues;
			protocol.CheckTrigger((int)trigger);
		}
	}
}
