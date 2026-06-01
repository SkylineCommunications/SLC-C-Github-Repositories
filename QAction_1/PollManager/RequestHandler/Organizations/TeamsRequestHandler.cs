namespace Skyline.Protocol.PollManager.RequestHandler.Organizations
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public static partial class OrganizationsRequestHandler
	{
		public static void HandleOrganizationsTeamsRequest(SLProtocol protocol, bool executeNow)
		{
			var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Organizations_Teams)?.PageLimit ?? PollingConstants.PerPage;
			HandleOrganizationTeamsRequest(protocol, perPage, 1, executeNow);
		}

		public static void HandleOrganizationTeamsRequest(SLProtocol protocol, int perPage, int page, bool executeNow)
		{
			var organizations = SLTables.Organizations.GetPrimaryKeys(protocol);
			foreach (var org in organizations)
			{
				HandleOrganizationTeamsRequest(protocol, org, perPage, page, executeNow);
			}
		}

		public static void HandleOrganizationTeamsRequest(SLProtocol protocol, string organization, int perPage, int page, bool executeNow)
		{
			protocol.SetParameter(Parameter.getorganizationteamsurl, $"orgs/{organization}/teams?per_page={perPage}&page={page}");
			var trigger = executeNow ? Triggers.GetOrganizationTeamsNow : Triggers.GetOrganizationTeams;
			protocol.CheckTrigger((int)trigger);
		}
	}
}
