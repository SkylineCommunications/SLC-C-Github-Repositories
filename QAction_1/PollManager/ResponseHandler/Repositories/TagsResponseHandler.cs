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
        public static void HandleRepositoriesTagsResponse(SLProtocol protocol)
        {
            // Check status code
            if (!protocol.IsSuccessStatusCode())
            {
                return;
            }

            // Parse response
            var response = JsonConvert.DeserializeObject<List<RepositoryTagsResponse>>(Convert.ToString(protocol.GetParameter(Parameter.getrepositorytagscontent)));

            if (response == null)
            {
                protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTagsResponse|response was null.", LogType.Error, LogLevel.NoLogging);
                return;
            }

            if (!response.Any())
            {
                // No tags for the repository
                return;
            }

            // Parse url to check which respository this issue is linked to
            var pattern = "https:\\/\\/api.github.com\\/repos\\/(.*)\\/(.*)\\/commits\\/(.*)";
            var options = RegexOptions.Multiline;

            var match = Regex.Match(response[0]?.Commit.Url, pattern, options);
            var owner = match.Groups[1].Value;
            var name = match.Groups[2].Value;

            // Update the tags table
            var table = new RepositoryTagsRecords();
            foreach (var tag in response)
            {
                if (tag == null)
                {
                    protocol.Log($"QA{protocol.QActionID}|GetRepositoryTagsResponse|Tag was null.", LogType.Information, LogLevel.NoLogging);
                    continue;
                }

                table.Rows.Add(new RepositoryTagsRecord
                {
                    ID = $"{owner}/{name}/commits/{tag.Name}",
                    Name = tag.Name,
                    RepositoryID = $"{owner}/{name}",
                    CommitSHA = tag.Commit?.Sha ?? Exceptions.NotAvailable,
                });
            }

            if (table.Rows.Count > 0)
            {
                table.SaveToProtocol(protocol, true);
            }

            protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTagsResponse|Tag repo: {owner}/{name}", LogType.DebugInfo, LogLevel.NoLogging);

            // Check if there are more tags to fetch
            var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositorytagslinkheader));
            if (string.IsNullOrEmpty(linkHeader)) return;

            var link = new LinkHeader(linkHeader);

            protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTagsResponse|Current page: {link.CurrentPage}", LogType.DebugInfo, LogLevel.NoLogging);
            protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTagsResponse|Has next page: {link.HasNext}", LogType.DebugInfo, LogLevel.NoLogging);

            if (link.HasNext)
            {
                RepositoriesRequestHandler.HandleRepositoriesTagsRequest(protocol, owner, name, PollingConstants.PerPage, link.NextPage);
            }
        }
    }
}
