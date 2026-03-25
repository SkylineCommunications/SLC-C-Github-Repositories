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
		public static void HandleRepositoriesTopicsRequest(SLProtocol protocol)
		{
			HandleRepositoriesTopicsRequest(protocol, PollingConstants.PerPage, 1);
		}

		public static void HandleRepositoriesTopicsRequest(SLProtocol protocol, int perPage, int page)
		{
			var rows = SLTables.Repositories.GetPrimaryKeys(protocol);
			if (!rows.Any())
			{
				return;
			}

			protocol.SetParameter(Parameter.getrepositorytopicsqueue, JsonConvert.SerializeObject(rows.Skip(1)));
			HandleRepositoriesTopicsRequest(protocol, rows[0], perPage, page);
		}

		public static void HandleRepositoriesTopicsRequest(SLProtocol protocol, string repositoryId, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getrepositoryissuesurl, $"repos/{repositoryId}/topics?per_page={perPage}&page={page}&state=all");
			protocol.CheckTrigger(229);
		}

		public static void CreateOrUpdateRepositoriesTopicsRequest(SLProtocol protocol, string repositoryId, IEnumerable<string> topics)
		{
			var request = new RepositoryTopics
			{
				Names = topics.ToList(),
			};

			protocol.SetParameter(Parameter.putrepositorytopicsurl, $"repos/{repositoryId}/topics");
			protocol.SetParameter(Parameter.putrepositorytopicsbody, JsonConvert.SerializeObject(request));
			protocol.CheckTrigger(230);
		}
	}
}
