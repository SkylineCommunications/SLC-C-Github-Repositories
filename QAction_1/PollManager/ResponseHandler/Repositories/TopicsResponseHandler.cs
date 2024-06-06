namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	using Newtonsoft.Json;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
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

			HandleTopicsInterApp(protocol, owner, name, repo.Topics);
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

		public static void HandleTopicsInterApp(SLProtocol protocol, string owner, string name, IEnumerable<string> topics)
		{
			// Check if there are Topics InterApp messages waiting on content creation
			var table = IAC_MessagesTable.GetTable(protocol);

			foreach (var iacRow in table.Rows.Where(iac => iac.ResponseType.AssemblyQualifiedName == typeof(AddRepositoryTopicsResponse).AssemblyQualifiedName))
			{
				// Check if for the given repo all the topics are added.
				var request = (GenericInterAppMessage<AddRepositoryTopicsRequest>)iacRow.Request;
				if (request.Data.RepositoryId.Owner == owner &&
					request.Data.RepositoryId.Name == name &&
					request.Data.Topics.All(topic => topics.Contains(topic)))
				{
					var returnMessage = (GenericInterAppMessage<AddRepositoryTopicsResponse>)iacRow.Response;
					returnMessage.Data.Success = true;
					returnMessage.Data.Description = $"Successfully added the following topics: {String.Join("\t", request.Data.Topics.Select(topic => $"'{topic}'"))}.";
					iacRow.Request.Reply(protocol.SLNet.RawConnection, returnMessage, Types.KnownTypes);
					iacRow.Status = IAC_MessageStatus.Confirmed;
					iacRow.SaveToProtocol(protocol);
				}
			}

			foreach (var iacRow in table.Rows.Where(iac => iac.ResponseType.AssemblyQualifiedName == typeof(RemoveRepositoryTopicsResponse).AssemblyQualifiedName))
			{
				// Check if for the given repo all the topics are removed.
				var request = (GenericInterAppMessage<RemoveRepositoryTopicsRequest>)iacRow.Request;
				if (request.Data.RepositoryId.Owner == owner &&
					request.Data.RepositoryId.Name == name &&
					!request.Data.Topics.Any(topic => topics.Contains(topic)))
				{
					var returnMessage = (GenericInterAppMessage<RemoveRepositoryTopicsResponse>)iacRow.Response;
					returnMessage.Data.Success = true;
					returnMessage.Data.Description = $"Successfully removed the following topics: {String.Join("\t", request.Data.Topics.Select(topic => $"'{topic}'"))}.";
					iacRow.Request.Reply(protocol.SLNet.RawConnection, returnMessage, Types.KnownTypes);
					iacRow.Status = IAC_MessageStatus.Confirmed;
					iacRow.SaveToProtocol(protocol);
				}
			}
		}
	}
}
