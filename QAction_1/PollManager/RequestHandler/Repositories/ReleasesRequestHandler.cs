namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesReleasesRequest(SLProtocol protocol, bool executeNow)
		{
			var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Repositories_Releases)?.PageLimit ?? PollingConstants.PerPage;
			HandleRepositoriesReleasesRequest(protocol, perPage, 1, executeNow);
		}

		public static void HandleRepositoriesReleasesRequest(SLProtocol protocol, int perPage, int page, bool executeNow)
		{
			var repositories = SLTables.Repositories.GetPrimaryKeys(protocol);
			foreach (var row in repositories)
			{
				HandleRepositoriesReleasesRequest(protocol, row, perPage, page, executeNow);
			}
		}

		public static void HandleRepositoriesReleasesRequest(SLProtocol protocol, string repositoryId, int perPage, int page, bool executeNow)
		{
			protocol.SetParameter(Parameter.getrepositoryreleasesurl, $"repos/{repositoryId}/releases?per_page={perPage}&page={page}");
			var trigger = executeNow ? Triggers.GetRepositoryReleasesNow : Triggers.GetRepositoryReleases;
			protocol.CheckTrigger((int)trigger);
		}
	}
}
