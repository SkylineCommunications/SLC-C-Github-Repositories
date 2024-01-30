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
		public static void HandleRepositoriesWorkflowsResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = JsonConvert.DeserializeObject<RepositoryWorkflowsResponse>(Convert.ToString(protocol.GetParameter(Parameter.getrepositoryworkflowscontent)));
			var url = Convert.ToString(protocol.GetParameter(Parameter.getrepositoryworkflowsurl));
			var table = RepositoryWorkflowsTable.GetTable();

			// Parse url to check which respository this issue is linked to
			var pattern = "repos\\/(.*)\\/(.*)\\/actions\\/workflows(.*)";
			var options = RegexOptions.Multiline;

			var match = Regex.Match(url, pattern, options);
			var owner = match.Groups[1].Value;
			var name = match.Groups[2].Value;

			// Sanity checks
			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			if (response.TotalCount <= 0)
			{
				// No workflows for the repository
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|No workflows for the repo.", LogType.Information, LogLevel.Level2);
				table.DeleteRow(protocol, table.Rows.Where(x => x.RepositoryID == $"{owner}/{name}").Select(x => x.ID).ToArray());
				HandleNextRepositoryWorkflow(protocol, owner, name);
				return;
			}

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

			HandleNextRepositoryWorkflow(protocol, owner, name);
		}

		private static void HandleNextRepositoryWorkflow(SLProtocol protocol, string owner, string name)
		{
			// Check if there are more workflows to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositoryworkflowslinkheader));
			var link = new LinkHeader(linkHeader);

			// Check if there are more workflows to fetch for the current repository
			if (!string.IsNullOrEmpty(linkHeader))
			{
				// Update the tags table
				if (link.IsFirst)
				{
					var table = RepositoryWorkflowsTable.GetTable();
					table.DeleteRow(protocol, table.Rows.Where(x => x.RepositoryID == $"{owner}/{name}").Select(x => x.ID).ToArray());
				}

				if (link.HasNext)
				{
					RepositoriesRequestHandler.HandleRepositoriesTagsRequest(protocol, owner, name, PollingConstants.PerPage, link.NextPage);
					return;
				}
			}

			// If no more workflows for this repo fetch the next repository in the queue.
			var queue = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(protocol.GetParameter(Parameter.getrepositoryworkflowsqueue)));
			var next = queue?.FirstOrDefault();

			if (next == null)
			{
				return;
			}

			protocol.SetParameter(Parameter.getrepositoryworkflowsqueue, JsonConvert.SerializeObject(queue.Skip(1)));

			var nextOwner = next.Split('/')[0];
			var nextName = next.Split('/')[1];
			RepositoriesRequestHandler.HandleRepositoriesWorkflowsRequest(protocol, nextOwner, nextName, PollingConstants.PerPage, 1);
		}
	}
}
