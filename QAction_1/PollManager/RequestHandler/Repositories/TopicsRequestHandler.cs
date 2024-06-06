namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Newtonsoft.Json;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;
	using System.Collections.Generic;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesTopicsRequest(SLProtocol protocol)
		{
			HandleRepositoriesTopicsRequest(protocol, PollingConstants.PerPage, 1);
		}

		public static void HandleRepositoriesTopicsRequest(SLProtocol protocol, int perPage, int page)
		{
			var table = RepositoriesTable.GetTable(protocol);
			var first = table.Rows.FirstOrDefault();
			if (first == null)
			{
				return;
			}

			protocol.SetParameter(Parameter.getrepositorytopicsqueue, JsonConvert.SerializeObject(table.Rows.Select(x => x.FullName).Skip(1)));
			HandleRepositoriesTopicsRequest(protocol, first.Owner, first.Name, perPage, page);
		}

		public static void HandleRepositoriesTopicsRequest(SLProtocol protocol, string owner, string name, int perPage, int page)
		{
			protocol.SetParameter(Parameter.getrepositoryissuesurl, $"repos/{owner}/{name}/topics?per_page={perPage}&page={page}&state=all");
			protocol.CheckTrigger(229);
		}

		public static void CreateOrUpdateRepositoriesTopicsRequest(SLProtocol protocol, string owner, string name, IEnumerable<string> topics)
		{
			var request = new RepositoryTopics
			{
				Names = topics.ToList(),
			};

			protocol.SetParameter(Parameter.putrepositorytopicsurl, $"repos/{owner}/{name}/topics");
			protocol.SetParameter(Parameter.putrepositorytopicsbody, JsonConvert.SerializeObject(request));
			protocol.CheckTrigger(230);
		}
	}
}
