namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesReleasesRequest(SLProtocol protocol, bool executeNow)
		{
			HandleRepositoriesReleasesRequest(protocol, 1, executeNow);
		}

		public static void HandleRepositoriesReleasesRequest(SLProtocol protocol, int page, bool executeNow)
		{
			var repositories = SLTables.Repositories.GetPrimaryKeys(protocol);
			foreach (var row in repositories)
			{
				HandleRepositoriesReleasesRequest(protocol, row, page, executeNow);
			}
		}

		public static void HandleRepositoriesReleasesRequest(SLProtocol protocol, string repositoryId, int page, bool executeNow)
		{
			var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Repositories_Releases)?.PageLimit ?? PollingConstants.PerPage;
			protocol.SetParameter(Parameter.getrepositoryreleasesurl, $"repos/{repositoryId}/releases?per_page={perPage}&page={page}");
			var trigger = executeNow ? Triggers.GetRepositoryReleasesNow : Triggers.GetRepositoryReleases;
			protocol.CheckTrigger((int)trigger);
		}
	}
}
