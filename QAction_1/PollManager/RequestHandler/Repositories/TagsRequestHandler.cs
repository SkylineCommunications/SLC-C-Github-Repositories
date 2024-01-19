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
			var table = RepositoriesTable.GetTable(protocol);
			foreach (var row in table.Rows)
			{
				HandleRepositoriesTagsRequest(protocol, row.Owner, row.Name, perPage, page);
			}
		}

		public static void HandleRepositoriesTagsRequest(SLProtocol protocol, string owner, string name, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getrepositorytagsurl, $"repos/{owner}/{name}/tags?per_page={perPage}&page={page}");
			protocol.CheckTrigger(203);
		}
	}
}
