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
		public static void HandleRepositoriesIssuesResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Repository_Issues, PollingStatus.Idle);
				return;
			}

			// Parse response
			var response = SecureNewtonsoftDeserialization.DeserializeObject<List<RepositoryIssuesResponse>>(
				Convert.ToString(protocol.GetParameter(Parameter.getrepositoryissuescontent_202)));
			if (response == null)
			{
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Repository_Issues, PollingStatus.Idle);
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			if (!response.Any())
			{
				// No issues for the repository
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Repository_Issues, PollingStatus.Idle);
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|No issues for the repo.", LogType.Information, LogLevel.Level2);
				return;
			}

			// Parse url to check which repository this issue is linked to
			GithubUrlHelper.TryParseRepoOwnerAndName(response[0].Url, out var owner, out var name);
			var repositoryId = $"{owner}/{name}";

			var utcNow = DateTime.UtcNow;

			// Update the issues table
			var rows = new List<RepositoryissuesQActionRow>();
			foreach (var issue in response)
			{
				// Update existing issue if found, otherwise create new one
				var id = $"{owner}/{name}/issues/{issue.Number}";
				var row = new IssuesModel
				{
					Instance = id,
					RepositoryID = $"{owner}/{name}",
					Number = issue.Number,
					Title = issue.Title,
					Body = issue.Body,
					Creator = issue.User.Login,
					State = (IssueState)Enum.Parse(typeof(IssueState), issue.State, true),
					Assignee = issue.Assignee?.Login,
					CreatedAt = issue.CreatedAt,
					UpdatedAt = issue.UpdatedAt,
					ClosedAt = issue.ClosedAt ?? default(DateTime),
					LastPolledAt = utcNow,
				};

				rows.Add(IssuesRowConverter.Instance.ToRawValue(row));
			}

			if (rows.Count > 0)
			{
				SLTables.Issues.FillTableNoDelete(protocol, rows);
			}

			// Check if there are more issues to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositoryissueslinkheader_252));
			var link = new LinkHeader(linkHeader);

			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|Current page: {link.CurrentPage}", LogType.Information, LogLevel.Level2);
			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|Has next page: {link.HasNext}", LogType.Information, LogLevel.Level2);

			if (link.HasNext)
			{
				var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Repository_Issues)?.PageLimit ?? PollingConstants.PerPage;
				RepositoriesRequestHandler.HandleRepositoriesIssuesRequest(protocol, repositoryId, perPage, link.NextPage, true);
			}
			else
			{
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Repository_Issues, PollingStatus.Idle);
				SLTables.Issues.Cleanup(protocol, repositoryId);
			}
		}
	}
}
