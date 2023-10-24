namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
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
		public static void HandleRepositoriesWorkflowsResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = JsonConvert.DeserializeObject<RepositoryWorkflowsResponse>(Convert.ToString(protocol.GetParameter(Parameter.getrepositoryworkflowscontent)));

			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|response was null.", LogType.Error, LogLevel.NoLogging);
				return;
			}

			if (response.TotalCount <= 0)
			{
				// No tags for the repository
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|No workflows for the repo.", LogType.Information, LogLevel.NoLogging);
				return;
			}

			// Parse url to check which respository this issue is linked to
			var pattern = "https:\\/\\/api.github.com\\/repos\\/(.*)\\/(.*)\\/actions\\/workflows\\/(.*)";
			var options = RegexOptions.Multiline;

			var match = Regex.Match(response.Workflows[0]?.Url, pattern, options);
			var owner = match.Groups[1].Value;
			var name = match.Groups[2].Value;

			// Update the tags table
			var table = new RepositoryWorkflowsRecords();
			foreach (var workflow in response.Workflows)
			{
				if (workflow == null)
				{
					protocol.Log($"QA{protocol.QActionID}|GetRepositoryWorkflowsResponse|Workflow was null.", LogType.Information, LogLevel.NoLogging);
					continue;
				}

				table.Rows.Add(new RepositoryWorkflowsRecord
				{
					ID = $"{owner}/{name}/actions/workflows/{workflow.Id}",
					RepositoryID = $"{owner}/{name}",
					Name = workflow.Name,
					State = workflow.State,
					Path = workflow.Path,
					CreatedAt = workflow.CreatedAt,
					UpdatedAt = workflow.UpdatedAt,
					DeletedAt = workflow.DeletedAt,
				});
			}

			if (table.Rows.Count > 0)
			{
				table.SaveToProtocol(protocol, true);
			}

			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|Workflow repo: {owner}/{name}", LogType.DebugInfo, LogLevel.NoLogging);

			// Check if there are more workflows to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositorytagslinkheader));
			if (string.IsNullOrEmpty(linkHeader)) return;

			var link = new LinkHeader(linkHeader);

			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|Current page: {link.CurrentPage}", LogType.DebugInfo, LogLevel.NoLogging);
			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|Has next page: {link.HasNext}", LogType.DebugInfo, LogLevel.NoLogging);

			if (link.HasNext)
			{
				RepositoriesRequestHandler.HandleRepositoriesTagsRequest(protocol, owner, name, PollingConstants.PerPage, link.NextPage);
			}
		}
	}
}
