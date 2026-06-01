namespace Skyline.Protocol.PollManager.RequestHandler.Organizations
{
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public static partial class OrganizationsRequestHandler
	{
		public static void HandleOrganizationsMembersRequest(SLProtocol protocol, bool executeNow)
		{
			var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Organizations_Members)?.PageLimit ?? PollingConstants.PerPage;
			HandleOrganizationMembersRequest(protocol, perPage, 1, executeNow);
		}

		public static void HandleOrganizationMembersRequest(SLProtocol protocol, int perPage, int page, bool executeNow)
		{
			var organizations = SLTables.Organizations.GetPrimaryKeys(protocol);
			if (!organizations.Any())
			{
				return;
			}

			protocol.SetParameter(Parameter.getorganizationmembersqueue, JsonConvert.SerializeObject(organizations.Skip(1)));
			HandleOrganizationMembersRequest(protocol, organizations[0], perPage, page, executeNow);
		}

		public static void HandleOrganizationMembersRequest(SLProtocol protocol, string organization, int perPage, int page, bool executeNow)
		{
			protocol.SetParameter(Parameter.getorganizationmembersurl, $"orgs/{organization}/members?per_page={perPage}&page={page}");
			var trigger = executeNow ? Triggers.GetOrganizationMembersNow : Triggers.GetOrganizationMembers;
			protocol.CheckTrigger((int)trigger);
		}
	}
}
