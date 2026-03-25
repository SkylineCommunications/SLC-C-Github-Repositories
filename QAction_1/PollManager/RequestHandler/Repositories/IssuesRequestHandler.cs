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
			var repositories = SLTables.Repositories.GetPrimaryKeys(protocol);
			foreach (var row in repositories)
			{
				HandleRepositoriesIssuesRequest(protocol, row, perPage, page);
			}
		}

		public static void HandleRepositoriesIssuesRequest(SLProtocol protocol, string repositoryId, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getrepositoryissuesurl, $"repos/{repositoryId}/issues?per_page={perPage}&page={page}&state=all");
			protocol.CheckTrigger(202);
		}
	}
}
