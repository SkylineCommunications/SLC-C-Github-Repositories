namespace Skyline.Protocol.PollManager.RequestHandler.Organizations
{
	using Newtonsoft.Json;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public static partial class OrganizationsRequestHandler
	{
		public static void HandleOrganizationsMembersRequest(SLProtocol protocol)
		{
			HandleOrganizationMembersRequest(protocol, PollingConstants.PerPage, 1);
		}

		public static void HandleOrganizationMembersRequest(SLProtocol protocol, int perPage, int page)
		{
			var table = OrganizationsTable.GetTable(protocol);
			var first = table.Rows[0];
			protocol.SetParameter(Parameter.getorganizationmembersqueue, JsonConvert.SerializeObject(table.Rows.Select(x => x.Instance).Skip(1)));
			HandleOrganizationMembersRequest(protocol, first.Instance, perPage, page);
		}

		public static void HandleOrganizationMembersRequest(SLProtocol protocol, string organization, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getorganizationmembersurl, $"orgs/{organization}/members?per_page={perPage}&page={page}");
			protocol.CheckTrigger(213);
		}
	}
}
