namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.Protocol;
	using Skyline.Protocol.API.Headers;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesResponseHandler
	{
		public static void HandleRepositoriesReleasesResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = JsonConvert.DeserializeObject<List<RepositoryReleasesResponse>>(Convert.ToString(protocol.GetParameter(Parameter.getrepositoryreleasescontent)));
			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryReleasesResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			if (!response.Any())
			{
				// No releases for the repository
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryReleasesResponse|No releases for the repo.", LogType.Information, LogLevel.Level2);
				return;
			}

			// Parse url to check which respository this issue is linked to
			var pattern = "https:\\/\\/api.github.com\\/repos\\/(.*)\\/(.*)\\/releases\\/(\\d+)";
			var options = RegexOptions.Multiline;

			var match = Regex.Match(response[0]?.Url, pattern, options);
			var owner = match.Groups[1].Value;
			var name = match.Groups[2].Value;

			// Update the releases table
			var table = RepositoryReleasesTable.GetTable();
			foreach (var release in response)
			{
				if (release == null)
				{
					protocol.Log($"QA{protocol.QActionID}|GetRepositoryReleasesResponse|Release was null.", LogType.Error, LogLevel.Level1);
					continue;
				}

				if (release.Url == null)
				{
					protocol.Log($"QA{protocol.QActionID}|GetRepositoryReleasesResponse|Release url null.", LogType.Error, LogLevel.Level1);
					continue;
				}

				// Update existing release if found, otherwise create new one
				var id = $"{owner}/{name}/releases/{release.Id}";
				var row = table.Rows.Find(rel => rel.Instance == id) ?? new RepositoryReleasesTableRow();
				row.RepositoryID = $"{owner}/{name}";
				row.ID = release.Id;
				row.TagName = release.TagName ?? Exceptions.NotAvailable;
				row.TagId = release.TagName != null ? $"{owner}/{name}/commits/{release.TagName}" : Exceptions.NotAvailable;
				row.TargetCommitish = release.TargetCommitish;
				row.Name = release.Name;
				row.Draft = release.Draft;
				row.PreRelease = release.Prerelease;
				row.Body = release.Body;
				row.Author = release.Author?.Login ?? Exceptions.NotAvailable;
				row.CreatedAt = release.CreatedAt;
				row.PublishedAt = release.PublishedAt;

				// If its a new row fill in ID and add it to the table.
				if (String.IsNullOrEmpty(row.Instance))
				{
					row.Instance = id;
					table.Rows.Add(row);
				}
			}

			if (table.Rows.Count > 0)
			{
				table.SaveToProtocol(protocol, true);
			}

			// Check if there are more releases to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositoryreleaseslinkheader));
			if (string.IsNullOrEmpty(linkHeader)) return;

			var link = new LinkHeader(linkHeader);

			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryReleasesResponse|Current page: {link.CurrentPage}", LogType.Information, LogLevel.Level2);
			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryReleasesResponse|Has next page: {link.HasNext}", LogType.Information, LogLevel.Level2);

			if (link.HasNext)
			{
				RepositoriesRequestHandler.HandleRepositoriesReleasesRequest(protocol, owner, name, PollingConstants.PerPage, link.NextPage);
			}
		}
	}
}
