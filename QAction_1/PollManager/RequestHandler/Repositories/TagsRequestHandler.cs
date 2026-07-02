namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesTagsRequest(SLProtocol protocol, bool executeNow)
		{
			var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Repositories_Tags)?.PageLimit ?? PollingConstants.PerPage;
			HandleRepositoriesTagsRequest(protocol, perPage, 1, executeNow);
		}

		public static void HandleRepositoriesTagsRequest(SLProtocol protocol, int perPage, int page, bool executeNow)
		{
			var rows = SLTables.Repositories.GetPrimaryKeys(protocol);
			foreach (var row in rows)
			{
				HandleRepositoriesTagsRequest(protocol, row, perPage, page, executeNow);
			}
		}

		public static void HandleRepositoriesTagsRequest(SLProtocol protocol, string repositoryId, int perPage, int page, bool executeNow)
		{
			protocol.SetParameter(Parameter.getrepositorytagsurl, $"repos/{repositoryId}/tags?per_page={perPage}&page={page}");
			var trigger = executeNow ? Triggers.GetRepositoryTagsNow : Triggers.GetRepositoryTags;
			protocol.CheckTrigger((int)trigger);
		}
	}
}
