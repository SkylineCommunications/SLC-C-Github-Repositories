namespace Skyline.Protocol.PollManager.RequestHandler.Organizations
{
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public static partial class OrganizationsRequestHandler
	{
		public static void HandleOrganizationRepositoriesRequest(SLProtocol protocol)
		{
			var table = OrganizationsTable.GetTable(protocol);
			foreach (var row in table.Rows.Where(org => org.Tracked))
			{
				HandleOrganizationRepositoriesRequest(protocol, row.Instance);
			}
		}

		public static void HandleOrganizationRepositoriesRequest(SLProtocol protocol, string organization)
		{
			HandleOrganizationRepositoriesRequest(protocol, organization, PollingConstants.PerPage, 1);
		}

		public static void HandleOrganizationRepositoriesRequest(SLProtocol protocol, string organization, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getorganizationrepositoriesurl, $"orgs/{organization}/repos?per_page={perPage}&page={page}");
			protocol.CheckTrigger(211);
		}
	}
}
