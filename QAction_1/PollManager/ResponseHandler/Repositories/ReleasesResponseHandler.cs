namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
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
		public static void HandleRepositoriesReleasesResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = SecureNewtonsoftDeserialization.DeserializeObject<List<RepositoryReleasesResponse>>(
				Convert.ToString(protocol.GetParameter(Parameter.getrepositoryreleasescontent)));
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

			// Parse url to check which repository this release is linked to
			GithubUrlHelper.TryParseRepoOwnerAndName(response[0]?.Url, out var owner, out var name);
			var repositoryId = $"{owner}/{name}";

			var utcNow = DateTime.UtcNow;

			// Update the releases table
			var rows = new List<RepositoryreleasesQActionRow>();
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
				var row = new ReleasesModel
				{
					Instance = id,
					RepositoryID = $"{owner}/{name}",
					ID = release.Id,
					TagName = release.TagName ?? Exceptions.NotAvailable,
					TagId = release.TagName != null ? $"{owner}/{name}/commits/{release.TagName}" : Exceptions.NotAvailable,
					TargetCommitish = release.TargetCommitish,
					Name = release.Name,
					Draft = release.Draft,
					PreRelease = release.Prerelease,
					Body = release.Body,
					Author = release.Author?.Login ?? Exceptions.NotAvailable,
					CreatedAt = release.CreatedAt,
					PublishedAt = release.PublishedAt,
					LastPolledAt = utcNow,
				};

				rows.Add(ReleasesRowConverter.Instance.ToRawValue(row));
			}

			if (rows.Count > 0)
			{
				SLTables.Releases.FillTableNoDelete(protocol, rows);
			}

			HandleRepositoryReleaseAssetsResponse(protocol, response, owner, name, repositoryId);

			// Check if there are more releases to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositoryreleaseslinkheader_254));
			var link = new LinkHeader(linkHeader);

			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryReleasesResponse|Current page: {link.CurrentPage}", LogType.Information, LogLevel.Level2);
			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryReleasesResponse|Has next page: {link.HasNext}", LogType.Information, LogLevel.Level2);

			if (link.HasNext)
			{
				RepositoriesRequestHandler.HandleRepositoriesReleasesRequest(protocol, repositoryId, link.NextPage, true);
			}
			else
			{
				SLTables.Releases.Cleanup(protocol, repositoryId);
				SLTables.ReleaseAssets.Cleanup(protocol, repositoryId);
			}
		}

		public static void HandleRepositoryReleaseAssetsResponse(SLProtocol protocol, List<RepositoryReleasesResponse> response, string owner, string name, string repositoryId)
		{
			var utcNow = DateTime.UtcNow;

			// Update the release assets table
			var rows = new List<RepositoryreleaseassetsQActionRow>();
			foreach (var release in response)
			{
				if (release?.Url == null)
				{
					protocol.Log($"QA{protocol.QActionID}|HandleRepositoryReleaseAssetsResponse|Release or Release url is null.", LogType.Error, LogLevel.Level1);
					continue;
				}

				foreach (var asset in release.Assets)
				{
					// Update existing release asset if found, otherwise create new one
					var id = $"{owner}/{name}/releases/{release.Id}/{asset.Id}";
					var row = new ReleaseAssetsModel
					{
						Instance = id,
						AssetId = asset.Id,
						RepositoryID = repositoryId,
						Release = $"{owner}/{name}/releases/{release.Id}",
						Uploader = asset.Uploader.Login,
						NodeID = asset.NodeId,
						Name = asset.Name,
						Label = asset.Label ?? Exceptions.NotAvailable,
						ContentType = asset.ContentType,
						State = asset.State,
						Size = asset.Size,
						DownloadCount = asset.DownloadCount,
						CreatedAt = asset.CreatedAt,
						UpdatedAt = asset.UpdatedAt,
						BrowserDownloadUrl = asset.BrowserDownloadUrl,
						LastPolledAt = utcNow,
					};

					rows.Add(ReleaseAssetsRowConverter.Instance.ToRawValue(row));
				}
			}

			if (rows.Count > 0)
			{
				SLTables.ReleaseAssets.FillTableNoDelete(protocol, rows);
			}
		}
	}
}
