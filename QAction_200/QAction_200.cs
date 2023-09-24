using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
using Skyline.Protocol;
using Skyline.Protocol.Extensions;
using Skyline.Protocol.Tables;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    private static IReadOnlyDictionary<int, Action<SLProtocol>> handlers = new Dictionary<int, Action<SLProtocol>>
    {
        { Parameter.getrepositorycontent, ParseGetRepositoryResponse },
        { Parameter.getrepositoryissuescontent, ParseGetRepositoryIssuesResponse },
        { Parameter.getrepositorytagscontent, ParseGetRepositoryTagsResponse },
        { Parameter.getrepositoryreleasescontent, ParseGetRepositoryReleasesResponse },
    };

    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    public static void Run(SLProtocol protocol)
    {
        try
        {
            var trigger = protocol.GetTriggerParameter();
            if(handlers.ContainsKey(trigger))
            {
                handlers[trigger](protocol);
            }
            else
            {
                protocol.Log($"QA{protocol.QActionID}|Run|No handler found for trigger parameter '{trigger}'", LogType.Error, LogLevel.NoLogging);
            }
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }

    public static void ParseGetRepositoryResponse(SLProtocol protocol)
    {
        if(!protocol.IsSuccessStatusCode())
        {
            return;
        }

        var response = JsonConvert.DeserializeObject<RepositoryResponse>(Convert.ToString(protocol.GetParameter(Parameter.getrepositorycontent)));
        var repo = new RepositoriesTableRow
        {
            Name = response.Name,
            FullName = response.FullName,
            Private = response.Private,
            Owner = response.Owner.Login,
            Description = response.Description,
            Fork = response.Fork,
            CreatedAt = response.CreatedAt,
            UpdatedAt = response.UpdatedAt,
            PushedAt = response.PushedAt,
            Size = response.Size,
            Stars = response.StargazersCount,
            Watcher = response.WatchersCount,
            Language = response.Language,
        };

        // Check if row exists, add or edit.
        var table = new RepositoriesTable(protocol);
        var exist = table.Rows.SingleOrDefault(row => row.FullName == response.FullName);
        if(exist == default(RepositoriesTableRow))
        {
            table.Rows.Add(repo);
        }
        else
        {
            var index = table.Rows.IndexOf(exist);
            table.Rows[index] = repo;
        }

        table.SaveToProtocol(protocol);
    }

    private static void ParseGetRepositoryIssuesResponse(SLProtocol protocol)
    {
        if (!protocol.IsSuccessStatusCode())
        {
            return;
        }

        var response = JsonConvert.DeserializeObject<List<RepositoryIssuesResponse>>(Convert.ToString(protocol.GetParameter(Parameter.getrepositoryissuescontent)));
        var pattern = "https:\\/\\/api.github.com\\/repos\\/(.*)\\/(.*)\\/issues\\/(\\d+)";
        var options = RegexOptions.Multiline;

        var table = new RepositoryIssuesTable(protocol);
        foreach (var issue in response)
        {
            // Parse url to check which respository this issue is linked to
            var match = Regex.Match(issue.Url, pattern, options);
            table.Rows.Add(new RepositoryIssuesRow
            {
                Instance = $"{match.Groups[1]}/{match.Groups[2]}/issues/{issue.Number}",
                Number = issue.Number,
                Title = issue.Title,
                Body = issue.Body,
                Creator = issue.User.Login,
                State = (IssueState)Enum.Parse(typeof(IssueState), issue.State, true),
                Assignee = issue.Assignee?.Login,
                CreatedAt = issue.CreatedAt,
                UpdatedAt = issue.UpdatedAt,
                ClosedAt = issue.ClosedAt ?? default(DateTime),
                RepositoryID = $"{match.Groups[1]}/{match.Groups[2]}",
            });
        }

        var toRemove = table.Rows.RemoveAll(row => String.IsNullOrEmpty(row.Title));
        if(toRemove > 0)
        {
            protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryIssuesResponse|Removed {toRemove} old rows.", LogType.Information, LogLevel.NoLogging);
        }

        table.SaveToProtocol(protocol);
    }

    private static void ParseGetRepositoryTagsResponse(SLProtocol protocol)
    {
        if (!protocol.IsSuccessStatusCode())
        {
            return;
        }

        var response = JsonConvert.DeserializeObject<List<RepositoryTagsResponse>>(Convert.ToString(protocol.GetParameter(Parameter.getrepositorytagscontent)));

        if(response == null)
        {
            protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryTagsResponse|response was null.", LogType.Error, LogLevel.NoLogging);
            return;
        }

        var pattern = "https:\\/\\/api.github.com\\/repos\\/(.*)\\/(.*)\\/commits\\/(.*)";
        var options = RegexOptions.Multiline;

        var table = new RepositoryTagsRecords(protocol);
        foreach (var tag in response)
        {
            if(tag == null)
            {
                protocol.Log($"QA{protocol.QActionID}|GetRepositoryTagsResponse|Tag was null.", LogType.Information, LogLevel.NoLogging);
                continue;
            }

            // Parse url to check which respository this issue is linked to
            var match = Regex.Match(tag?.Commit.Url, pattern, options);
            table.Rows.Add(new RepositoryTagsRecord
            {
                ID = $"{match.Groups[1]}/{match.Groups[2]}/commits/{tag.Name}",
                Name = tag.Name,
                RepositoryID = $"{match.Groups[1]}/{match.Groups[2]}",
                CommitSHA = tag.Commit?.Sha ?? "-2",
            });
        }

        table.SaveToProtocol(protocol);
    }

    private static void ParseGetRepositoryReleasesResponse(SLProtocol protocol)
    {
        if (!protocol.IsSuccessStatusCode())
        {
            return;
        }

        var response = JsonConvert.DeserializeObject<List<RepositoryReleasesResponse>>(Convert.ToString(protocol.GetParameter(Parameter.getrepositoryreleasescontent)));

        if (response == null)
        {
            protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryReleasesResponse|response was null.", LogType.Error, LogLevel.NoLogging);
            return;
        }

        var pattern = "https:\\/\\/api.github.com\\/repos\\/(.*)\\/(.*)\\/releases\\/(\\d+)";
        var options = RegexOptions.Multiline;

        var table = new RepositoryReleasesRecords(protocol);
        foreach (var release in response)
        {
            if (release == null)
            {
                protocol.Log($"QA{protocol.QActionID}|GetRepositoryReleasesResponse|Release was null.", LogType.Information, LogLevel.NoLogging);
                continue;
            }

            if(release.Url == null)
            {
                protocol.Log($"QA{protocol.QActionID}|GetRepositoryReleasesResponse|Release url null.", LogType.Information, LogLevel.NoLogging);
                continue;
            }

            // Parse url to check which respository this issue is linked to
            var match = Regex.Match(release.Url, pattern, options);
            table.Rows.Add(new RepositoryReleasesRecord
            {
                Instance = $"{match.Groups[1]}/{match.Groups[2]}/releases/{release.Id}",
                ID = release.Id,
                TagName = release.TagName ?? Exceptions.NotAvailable,
                TagId = release.TagName != null ? $"{match.Groups[1]}/{match.Groups[2]}/commits/{release.TagName}" : Exceptions.NotAvailable,
                TargetCommitish = release.TargetCommitish,
                Name = release.Name,
                Draft = release.Draft,
                PreRelease = release.Prerelease,
                Body = release.Body,
                Author = release.Author?.Login ?? Exceptions.NotAvailable,
                CreatedAt = release.CreatedAt,
                PublishedAt = release.PublishedAt,
                RepositoryId = $"{match.Groups[1]}/{match.Groups[2]}",
            });
        }

        table.SaveToProtocol(protocol);
    }
}
