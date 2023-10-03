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
                protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|response was null.", LogType.Error, LogLevel.NoLogging);
                return;
            }

            if (!response.Any())
            {
                // No issues for the repository
                return;
            }

            // Parse url to check which respository this issue is linked to
            var pattern = "https:\\/\\/api.github.com\\/repos\\/(.*)\\/(.*)\\/issues\\/(\\d+)";
            var options = RegexOptions.Multiline;

            var match = Regex.Match(response[0].Url, pattern, options);
            var owner = match.Groups[1].Value;
            var name = match.Groups[2].Value;

            // Update the issues table
            var table = new RepositoryIssuesTable();
            foreach (var issue in response)
            {
                table.Rows.Add(new RepositoryIssuesRow
                {
                    Instance = $"{owner}/{name}/issues/{issue.Number}",
                    Number = issue.Number,
                    Title = issue.Title,
                    Body = issue.Body,
                    Creator = issue.User.Login,
                    State = (IssueState)Enum.Parse(typeof(IssueState), issue.State, true),
                    Assignee = issue.Assignee?.Login,
                    CreatedAt = issue.CreatedAt,
                    UpdatedAt = issue.UpdatedAt,
                    ClosedAt = issue.ClosedAt ?? default(DateTime),
                    RepositoryID = $"{owner}/{name}",
                });
            }

            if (table.Rows.Count > 0)
            {
                table.SaveToProtocol(protocol, true);
            }

            protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|Issue repo: {owner}/{name}", LogType.DebugInfo, LogLevel.NoLogging);

            // Check if there are more tags to fetch
            var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositoryissueslinkheader));
            if (string.IsNullOrEmpty(linkHeader)) return;

            var link = new LinkHeader(linkHeader);

            protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|Current page: {link.CurrentPage}", LogType.DebugInfo, LogLevel.NoLogging);
            protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|Has next page: {link.HasNext}", LogType.DebugInfo, LogLevel.NoLogging);

            if (link.HasNext)
            {
                RepositoriesRequestHandler.HandleRepositoriesIssuesRequest(protocol, owner, name, PollingConstants.PerPage, link.NextPage);
            }
        }
    }
}
