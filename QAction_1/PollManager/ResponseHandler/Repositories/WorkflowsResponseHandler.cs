namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.IO;
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

	using static SLDataGateway.API.Types.Migration.ETLStatus;

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
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			if (response.TotalCount <= 0)
			{
				// No workflows for the repository
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|No workflows for the repo.", LogType.Information, LogLevel.Level2);
				return;
			}

			// Parse url to check which respository this issue is linked to
			var pattern = "https:\\/\\/api.github.com\\/repos\\/(.*)\\/(.*)\\/actions\\/workflows\\/(.*)";
			var options = RegexOptions.Multiline;

			var match = Regex.Match(response.Workflows[0]?.Url, pattern, options);
			var owner = match.Groups[1].Value;
			var name = match.Groups[2].Value;

			// Update the tags table
			var table = RepositoryWorkflowsTable.GetTable();
			foreach (var workflow in response.Workflows)
			{
				if (workflow == null)
				{
					protocol.Log($"QA{protocol.QActionID}|GetRepositoryWorkflowsResponse|Workflow was null.", LogType.Information, LogLevel.NoLogging);
					continue;
				}

				// Update existing workflow if found, otherwise create new one
				var id = $"{owner}/{name}/actions/workflows/{workflow.Id}";
				var row = table.Rows.Find(wf => wf.ID == id) ?? new RepositoryWorkflowsTableRow();
				row.RepositoryID = $"{owner}/{name}";
				row.Name = workflow.Name;
				row.State = workflow.State;
				row.Path = workflow.Path;
				row.CreatedAt = workflow.CreatedAt;
				row.UpdatedAt = workflow.UpdatedAt;
				row.DeletedAt = workflow.DeletedAt;

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

			// Check if there are more workflows to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositoryworkflowslinkheader));
			if (string.IsNullOrEmpty(linkHeader)) return;

			var link = new LinkHeader(linkHeader);

			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|Current page: {link.CurrentPage}", LogType.Information, LogLevel.Level2);
			protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|Has next page: {link.HasNext}", LogType.Information, LogLevel.Level2);

			if (link.HasNext)
			{
				RepositoriesRequestHandler.HandleRepositoriesTagsRequest(protocol, owner, name, PollingConstants.PerPage, link.NextPage);
			}
		}
	}
}
