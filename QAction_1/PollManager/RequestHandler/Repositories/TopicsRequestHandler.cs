namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesTopicsRequest(SLProtocol protocol, bool executeNext)
		{
			var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Repositories_Topics)?.PageLimit ?? PollingConstants.PerPage;
			HandleRepositoriesTopicsRequest(protocol, perPage, 1, executeNext);
		}

		public static void HandleRepositoriesTopicsRequest(SLProtocol protocol, int perPage, int page, bool executeNext)
		{
			var rows = SLTables.Repositories.GetPrimaryKeys(protocol);
			if (!rows.Any())
			{
				return;
			}

			protocol.SetParameter(Parameter.getrepositorytopicsqueue, JsonConvert.SerializeObject(rows.Skip(1)));
			HandleRepositoriesTopicsRequest(protocol, rows[0], perPage, page, executeNext);
		}

		public static void HandleRepositoriesTopicsRequest(SLProtocol protocol, string repositoryId, int perPage, int page, bool executeNext)
		{
			protocol.SetParameter(Parameter.getrepositoryissuesurl, $"repos/{repositoryId}/topics?per_page={perPage}&page={page}&state=all");
			var trigger = executeNext ? Triggers.GetRepositoryTopicsNow : Triggers.GetRepositoryTopics;
			protocol.CheckTrigger((int)trigger);
		}

		public static void CreateOrUpdateRepositoriesTopicsRequest(SLProtocol protocol, string repositoryId, IEnumerable<string> topics)
		{
			var request = new RepositoryTopics
			{
				Names = topics.ToList(),
			};

			var paramsToSet = new Dictionary<int, object>
			{
				{ Parameter.putrepositorytopicsurl, $"repos/{repositoryId}/topics" },
				{ Parameter.putrepositorytopicsbody, JsonConvert.SerializeObject(request) },
			};

			protocol.SetParameters(paramsToSet.Keys.ToArray(), paramsToSet.Values.ToArray());
			protocol.CheckTrigger((int)Triggers.PutRepositoryTopicsNow);
		}
	}
}
