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
		public static void HandleRepositoriesIssuesResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = JsonConvert.DeserializeObject<List<RepositoryIssuesResponse>>(Convert.ToString(protocol.GetParameter(Parameter.getrepositoryissuescontent)));
			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			if (!response.Any())
			{
				// No issues for the repository
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|No issues for the repo.", LogType.Information, LogLevel.Level2);
				return;
			}

			// Parse url to check which respository this issue is linked to
			var pattern = "https:\\/\\/api.github.com\\/repos\\/(.*)\\/(.*)\\/issues\\/(\\d+)";
			var options = RegexOptions.Multiline;

			var match = Regex.Match(response[0].Url, pattern, options);
			var owner = match.Groups[1].Value;
			var name = match.Groups[2].Value;

			// Update the issues table
			var table = RepositoryIssuesTable.GetTable();
			foreach (var issue in response)
			{
				// Update existing issue if found, otherwise create new one
				var id = $"{owner}/{name}/issues/{issue.Number}";
				var row = table.Rows.Find(iss => iss.Instance == id) ?? new RepositoryIssuesRow();
				row.RepositoryID = $"{owner}/{name}";
				row.Number = issue.Number;
				row.Title = issue.Title;
				row.Body = issue.Body;
				row.Creator = issue.User.Login;
				row.State = (IssueState)Enum.Parse(typeof(IssueState), issue.State, true);
				row.Assignee = issue.Assignee?.Login;
				row.CreatedAt = issue.CreatedAt;
				row.UpdatedAt = issue.UpdatedAt;
				row.ClosedAt = issue.ClosedAt ?? default(DateTime);

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

			// Check if there are more tags to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositoryissueslinkheader));
			if (string.IsNullOrEmpty(linkHeader)) return;

			var link = new LinkHeader(linkHeader);

			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|Current page: {link.CurrentPage}", LogType.Information, LogLevel.Level2);
			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|Has next page: {link.HasNext}", LogType.Information, LogLevel.Level2);

			if (link.HasNext)
			{
				RepositoriesRequestHandler.HandleRepositoriesIssuesRequest(protocol, owner, name, PollingConstants.PerPage, link.NextPage);
			}
		}
	}
}
