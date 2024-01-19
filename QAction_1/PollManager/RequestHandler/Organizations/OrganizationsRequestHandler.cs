namespace Skyline.Protocol.PollManager.RequestHandler.Organizations
{
	using Skyline.DataMiner.Scripting;

	public static partial class OrganizationsRequestHandler
	{
		public static void HandleUserOrganizationsRequest(SLProtocol protocol)
		{
			HandleUserOrganizationsRequest(protocol, PollingConstants.PerPage, 1);
		}

		public static void HandleUserOrganizationsRequest(SLProtocol protocol, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getuserorganizationsurl, $"user/orgs?per_page={perPage}&page={page}");
			protocol.CheckTrigger(210);
		}
	}
}
