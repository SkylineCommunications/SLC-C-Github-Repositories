namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
	using Skyline.Protocol;
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
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositorytagslinkheader_253));
			var link = new LinkHeader(linkHeader);

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

			// Parse url to check which respository this issue is linked to
			var pattern = "https:\\/\\/api.github.com\\/repos\\/(.*)\\/(.*)\\/commits\\/(.*)";
			var options = RegexOptions.Multiline;

			var utcNow = DateTime.UtcNow;
			var match = Regex.Match(response[0]?.Commit.Url, pattern, options);
			var owner = match.Groups[1].Value;
			var name = match.Groups[2].Value;
			var repositoryId = $"{owner}/{name}";

			// Update the tags table
			var table = RepositoryTagsTable.GetTable();
			foreach (var tag in response)
			{
				if (tag == null)
				{
					protocol.Log($"QA{protocol.QActionID}|GetRepositoryTagsResponse|Tag was null.", LogType.Error, LogLevel.Level1);
					continue;
				}

				// Update existing workflow if found, otherwise create new one
				var id = $"{owner}/{name}/commits/{tag.Name}";
				var row = table.Rows.Find(wf => wf.ID == id) ?? new RepositoryTagsTableRow();
				row.Name = tag.Name;
				row.RepositoryID = repositoryId;
				row.CommitSHA = tag.Commit?.Sha ?? Exceptions.NotAvailable;
				row.LastPolledAt = utcNow;

				// If its a new row fill in ID and add it to the table.
				if (String.IsNullOrEmpty(row.ID))
				{
					row.ID = id;
					table.Rows.Add(row);
				}
			}

			if (table.Rows.Count > 0)
			{
				table.SaveToProtocol(protocol, true);
			}

			// Check if there are more tags to fetch
			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTagsResponse|Current page: {link.CurrentPage}", LogType.Information, LogLevel.Level2);
			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTagsResponse|Has next page: {link.HasNext}", LogType.Information, LogLevel.Level2);

			if (link.HasNext)
			{
				RepositoriesRequestHandler.HandleRepositoriesTagsRequest(protocol, owner, name, PollingConstants.PerPage, link.NextPage);
			}
			else
			{
				RepositoryTagsTable.GetTable(protocol).Cleanup(protocol, repositoryId);
			}
		}
	}
}
