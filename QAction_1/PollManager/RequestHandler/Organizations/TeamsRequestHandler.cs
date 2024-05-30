namespace Skyline.Protocol.PollManager.RequestHandler.Organizations
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public static partial class OrganizationsRequestHandler
	{
		public static void HandleOrganizationsTeamsRequest(SLProtocol protocol)
		{
			HandleOrganizationTeamsRequest(protocol, PollingConstants.PerPage, 1);
		}

		public static void HandleOrganizationTeamsRequest(SLProtocol protocol, int perPage, int page)
		{
			var table = OrganizationsTable.GetTable(protocol);
			foreach (var org in table.Rows)
			{
				HandleOrganizationTeamsRequest(protocol, org.Instance, perPage, page);
			}
		}

		public static void HandleOrganizationTeamsRequest(SLProtocol protocol, string organization, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getorganizationteamsurl, $"orgs/{organization}/teams?per_page={perPage}&page={page}");
			protocol.CheckTrigger(212);
		}
	}
}
