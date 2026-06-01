namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
	using Skyline.Protocol;
	using Skyline.Protocol.API;
	using Skyline.Protocol.API.Headers;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesResponseHandler
	{
		public static void HandleRepositoriesTagsResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = SecureNewtonsoftDeserialization.DeserializeObject<List<RepositoryTagsResponse>>(
				Convert.ToString(protocol.GetParameter(Parameter.getrepositorytagscontent)));
			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTagsResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			if (!response.Any())
			{
				// No tags for the repository
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTagsResponse|No tags for the repo.", LogType.Information, LogLevel.Level2);
				return;
			}

			// Parse url to check which repository this tag is linked to
			GithubUrlHelper.TryParseRepoOwnerAndName(response[0]?.Commit.Url, out var owner, out var name);
			var repositoryId = $"{owner}/{name}";

			var utcNow = DateTime.UtcNow;

			// Update the tags table
			var rows = new List<RepositorytagsQActionRow>();
			foreach (var tag in response)
			{
				if (tag == null)
				{
					protocol.Log($"QA{protocol.QActionID}|GetRepositoryTagsResponse|Tag was null.", LogType.Error, LogLevel.Level1);
					continue;
				}

				// Update existing tag if found, otherwise create new one
				var id = $"{owner}/{name}/commits/{tag.Name}";
				var row = new TagsModel
				{
					ID = id,
					Name = tag.Name,
					RepositoryID = repositoryId,
					CommitSHA = tag.Commit?.Sha ?? Exceptions.NotAvailable,
					LastPolledAt = utcNow,
				};

				rows.Add(TagsRowConverter.Instance.ToRawValue(row));
			}

			if (rows.Count > 0)
			{
				SLTables.Tags.FillTableNoDelete(protocol, rows);
			}

			// Check if there are more tags to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositorytagslinkheader_253));
			var link = new LinkHeader(linkHeader);

			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTagsResponse|Current page: {link.CurrentPage}", LogType.Information, LogLevel.Level2);
			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTagsResponse|Has next page: {link.HasNext}", LogType.Information, LogLevel.Level2);

			if (link.HasNext)
			{
				var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Repositories_Tags)?.PageLimit ?? PollingConstants.PerPage;
				RepositoriesRequestHandler.HandleRepositoriesTagsRequest(protocol, repositoryId, perPage, link.NextPage, true);
			}
			else
			{
				SLTables.Tags.Cleanup(protocol, repositoryId);
			}
		}
	}
}
