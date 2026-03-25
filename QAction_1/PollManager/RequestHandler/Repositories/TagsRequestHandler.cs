namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesTagsRequest(SLProtocol protocol)
		{
			HandleRepositoriesTagsRequest(protocol, PollingConstants.PerPage, 1);
		}

		public static void HandleRepositoriesTagsRequest(SLProtocol protocol, int perPage, int page)
		{
			var rows = SLTables.Repositories.GetPrimaryKeys(protocol);
			foreach (var row in rows)
			{
				HandleRepositoriesTagsRequest(protocol, row, perPage, page);
			}
		}

		public static void HandleRepositoriesTagsRequest(SLProtocol protocol, string repositoryId, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getrepositorytagsurl, $"repos/{repositoryId}/tags?per_page={perPage}&page={page}");
			protocol.CheckTrigger(203);
		}
	}
}
