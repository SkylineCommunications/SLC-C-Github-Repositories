namespace Skyline.Protocol.PollManager.RequestHandler.Organizations
{
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Organizations;
	using Skyline.DataMiner.Utils.Protocol.Extension;
	using Skyline.Protocol.Tables;

	public static partial class OrganizationsRequestHandler
	{
		public static void HandleOrganizationRepositoriesRequest(SLProtocol protocol, bool executeNow)
		{
			var rows = SLTables.Organizations.GetData(
				protocol,
				SLTables.Organizations.Instance.Read.Map<OrganizationsModel>(m => m.Instance),
				SLTables.Organizations.Tracked.Read.Map<OrganizationsModel>(m => m.Tracked));

			var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Organizations_Repositories)?.PageLimit ?? PollingConstants.PerPage;
			var orgsToPoll = rows.Where(org => org.Tracked.HasValue && org.Tracked.Value).Select(org => org.Instance).ToList();
			var orgToPoll = orgsToPoll.FirstOrDefault();
			if (orgToPoll == null)
			{
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Organizations_Repositories, PollingStatus.Idle);
			}

			orgsToPoll.Remove(orgToPoll);
			protocol.SetParameter(Parameter.organizationrepositoriespollingqueue_998, JsonConvert.SerializeObject(orgsToPoll));

			HandleOrganizationRepositoriesRequest(protocol, orgToPoll, perPage, executeNow);
		}

		public static void HandleOrganizationRepositoriesRequest(SLProtocol protocol, string organization, int perPage, bool executeNow)
		{
			HandleOrganizationRepositoriesRequest(protocol, organization, perPage, 1, executeNow);
		}

		public static void HandleOrganizationRepositoriesRequest(SLProtocol protocol, string organization, int perPage, int page, bool executeNow)
		{
			protocol.SetParameter(Parameter.getorganizationrepositoriesurl, $"orgs/{organization}/repos?per_page={perPage}&page={page}");
			var trigger = executeNow ? Triggers.GetOrganizationRepositoriesNow : Triggers.GetOrganizationRepositories;
			protocol.CheckTrigger((int)trigger);
		}

		public static void HandleOrganizationCreateRepositoryRequest(SLProtocol protocol, string organization, CreateRepository repo)
		{
			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
			};

			var sets = new Dictionary<int, object>
			{
				{ Parameter.postrepositoryurl,              $"orgs/{organization}/repos" },
				{ Parameter.postrepositorybody,             JsonConvert.SerializeObject(repo, settings) },
			};

			protocol.SetParameters(sets.Keys.ToArray(), sets.Values.ToArray());
			protocol.CheckTrigger((int)Triggers.PostRepositoryNow);
		}
	}
}
