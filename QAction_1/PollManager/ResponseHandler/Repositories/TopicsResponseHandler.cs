namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
	using Skyline.Protocol.API;
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

			var parameterIds = new uint[]
			{
				Parameter.getrepositorytopicscontent,
				Parameter.getrepositorytopicsurl,
			};

			var parameterValues = Array.ConvertAll((object[])protocol.GetParameters(parameterIds), Convert.ToString);
			var response = SecureNewtonsoftDeserialization.DeserializeObject<RepositoryTopics>(parameterValues[0]);
			var url = parameterValues[1];

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

			var parameterIds = new uint[]
			{
				Parameter.putrepositorytopicscontent,
				Parameter.putrepositorytopicsurl,
			};

			var parameterValues = Array.ConvertAll((object[])protocol.GetParameters(parameterIds), Convert.ToString);
			var response = SecureNewtonsoftDeserialization.DeserializeObject<RepositoryTopics>(parameterValues[0]);
			var url = parameterValues[1];

			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParsePutRepositoryTopicsResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			HandleRepositoriesTopicsResponse(protocol, response, url);
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

		private static void HandleRepositoriesTopicsResponse(SLProtocol protocol, RepositoryTopics response, string url)
		{
			// Parse url to check which repository this topic is linked to
			if (!GithubUrlHelper.TryParseRepoOwnerAndName(url, out var owner, out var name))
			{
				protocol.Log($"QA{protocol.QActionID}|HandleRepositoriesTopicsResponse|Did not find a tracked repository for this request.", LogType.Information, LogLevel.Level1);
				return;
			}

			if (!SLTables.Repositories.TryGetRow(protocol, $"{owner}/{name}", out var rawRow))
			{
				return;
			}

			// Update the repositories table
			var repo = RepositoriesRowConverter.Instance.FromRawValue(rawRow);
			if (repo == null)
			{
				return;
			}

			repo.Topics.Clear();
			repo.Topics.AddRange(response.Names);
			SLTables.Repositories.SetRow(protocol, RepositoriesRowConverter.Instance.ToRawValue(repo));

			HandleTopicsInterApp(protocol, owner, name, repo.Topics);
		}

		private static void HandleNextRepositoryTopics(SLProtocol protocol)
		{
			// Get the next repo in the queue to fetch
			var queue = SecureNewtonsoftDeserialization.DeserializeObject<List<string>>(
				Convert.ToString(protocol.GetParameter(Parameter.getrepositorytopicsqueue)));
			var next = queue?.FirstOrDefault();

			if (next == null)
			{
				return;
			}

			protocol.SetParameter(Parameter.getrepositorytopicsqueue, JsonConvert.SerializeObject(queue.Skip(1)));
			var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Repositories_Topics)?.PageLimit ?? PollingConstants.PerPage;
			RepositoriesRequestHandler.HandleRepositoriesTopicsRequest(protocol, next, perPage, 1, true);
		}
	}
}
