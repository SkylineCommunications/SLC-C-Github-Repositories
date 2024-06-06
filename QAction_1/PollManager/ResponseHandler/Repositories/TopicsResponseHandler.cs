namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Security.Policy;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesResponseHandler
	{
		public static void HandleRepositoriesTopicsResponse(SLProtocol protocol)
		{
			// Check status code
			var code = protocol.GetStatusCode();
			if (code >= 400 && code <= 499)
			{
				HandleNextRepositoryTopics(protocol);
				return;
			}

			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = JsonConvert.DeserializeObject<RepositoryTopics>(Convert.ToString(protocol.GetParameter(Parameter.getrepositorytopicscontent)));
			var url = Convert.ToString(protocol.GetParameter(Parameter.getrepositorytopicsurl));

			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTopicsResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			HandleRepositoriesTopicsResponse(protocol, response, url);
			HandleNextRepositoryTopics(protocol);
		}

		public static void HandleRepositoriesCreateOrUpdateTopicsResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = JsonConvert.DeserializeObject<RepositoryTopics>(Convert.ToString(protocol.GetParameter(Parameter.putrepositorytopicscontent)));
			var url = Convert.ToString(protocol.GetParameter(Parameter.putrepositorytopicsurl));

			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParsePutRepositoryTopicsResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			HandleRepositoriesTopicsResponse(protocol, response, url);
		}

		private static void HandleRepositoriesTopicsResponse(SLProtocol protocol, RepositoryTopics response, string url)
		{
			// Parse url to check which respository this issue is linked to
			var pattern = "repos\\/(.*)\\/(.*)\\/topics";
			var options = RegexOptions.Multiline;

			var match = Regex.Match(url, pattern, options);
			if (!match.Success)
			{
				protocol.Log($"QA{protocol.QActionID}|HandleRepositoriesTopicsResponse|Did not find a tracked repository for this request.", LogType.Information, LogLevel.Level1);
				return;
			}

			var owner = match.Groups[1].Value;
			var name = match.Groups[2].Value;

			// Update the repositories table
			var repo = RepositoriesTable.GetTable().Rows.Find(x => x.FullName == $"{owner}/{name}");
			if (repo == null)
			{
				return;
			}

			repo.Topics.Clear();
			repo.Topics.AddRange(response.Names);
			repo.SaveToProtocol(protocol);
		}

		private static void HandleNextRepositoryTopics(SLProtocol protocol)
		{
			// Get the next repo in the queue to fetch
			var queue = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(protocol.GetParameter(Parameter.getrepositorytopicsqueue)));
			var next = queue?.FirstOrDefault();

			if (next == null)
			{
				return;
			}

			protocol.SetParameter(Parameter.getrepositorytopicsqueue, JsonConvert.SerializeObject(queue.Skip(1)));

			var nextOwner = next.Split('/')[0];
			var nextName = next.Split('/')[1];
			RepositoriesRequestHandler.HandleRepositoriesTopicsRequest(protocol, nextOwner, nextName, PollingConstants.PerPage, 1);
		}
	}
}
