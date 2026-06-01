namespace Skyline.Protocol.PollManager.RequestHandler.Organizations
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public static partial class OrganizationsRequestHandler
	{
		public static void HandleUserOrganizationsRequest(SLProtocol protocol, bool executeNow)
		{
			var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Organizations_User)?.PageLimit ?? PollingConstants.PerPage;
			HandleUserOrganizationsRequest(protocol, perPage, 1, executeNow);
		}

		public static void HandleUserOrganizationsRequest(SLProtocol protocol, int perPage, int page, bool executeNow)
		{
			protocol.SetParameter(Parameter.getuserorganizationsurl, $"user/orgs?per_page={perPage}&page={page}");
			var trigger = executeNow ? Triggers.GetUserOrganizationsNow : Triggers.GetUserOrganizations;
			protocol.CheckTrigger((int)trigger);
		}
	}
}
