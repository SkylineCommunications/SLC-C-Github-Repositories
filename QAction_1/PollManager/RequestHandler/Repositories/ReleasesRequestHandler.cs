namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesReleasesRequest(SLProtocol protocol)
		{
			HandleRepositoriesReleasesRequest(protocol, PollingConstants.PerPage, 1);
		}

		public static void HandleRepositoriesReleasesRequest(SLProtocol protocol, int perPage, int page)
		{
			var repositories = SLTables.Repositories.GetPrimaryKeys(protocol);
			foreach (var row in repositories)
			{
				HandleRepositoriesReleasesRequest(protocol, row, perPage, page);
			}
		}

		public static void HandleRepositoriesReleasesRequest(SLProtocol protocol, string repositoryId, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getrepositoryreleasesurl, $"repos/{repositoryId}/releases?per_page={perPage}&page={page}");
			protocol.CheckTrigger(204);
		}
	}
}
