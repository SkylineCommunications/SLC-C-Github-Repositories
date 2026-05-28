// Ignore Spelling: Workflows

namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Web;

	using Newtonsoft.Json;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
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
		private static readonly Regex WorkflowDispatchUrlRegex = new Regex(
			@"repos/([^/]+)/([^/]+)/actions/workflows/(.*)/dispatches",
			RegexOptions.Compiled);

		public static void HandleRepositoriesWorkflowsResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				HandleNextRepositoryWorkflow(protocol);
				return;
			}

			// Parse response
			var response = SecureNewtonsoftDeserialization.DeserializeObject<RepositoryWorkflowsResponse>(
				Convert.ToString(protocol.GetParameter(Parameter.getrepositoryworkflowscontent_205)));
			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|response was null.", LogType.Error, LogLevel.Level1);
				HandleNextRepositoryWorkflow(protocol);
				return;
			}

			// Parse url to check which repository this workflow is linked to
			var url = Convert.ToString(protocol.GetParameter(Parameter.getrepositoryworkflowsurl_105));
			GithubUrlHelper.TryParseRepoOwnerAndName(url, out var owner, out var name);
			var repositoryId = $"{owner}/{name}";
			var primaryKeys = SLTables.Workflows.RepositoryID.Read.GetPrimaryKeysForValue(protocol, repositoryId);

			var utcNow = DateTime.UtcNow;

			if (response.TotalCount <= 0)
			{
				// No workflows for the repository
				protocol.Log($"QA{protocol.QActionID}|ParseGetRepositoryWorkflowsResponse|No workflows for the repo.", LogType.Information, LogLevel.Level2);
				SLTables.Workflows.DeleteRows(protocol, primaryKeys);
				HandleNextRepositoryWorkflow(protocol);
				return;
			}

			var rows = new List<RepositoryworkflowsQActionRow>();
			foreach (var workflow in response.Workflows)
			{
				if (workflow == null)
				{
					protocol.Log($"QA{protocol.QActionID}|GetRepositoryWorkflowsResponse|Workflow was null.", LogType.Information, LogLevel.NoLogging);
					continue;
				}

				// Update existing workflow if found, otherwise create new one
				var id = $"{owner}/{name}/actions/workflows/{workflow.Id}";
				var row = new WorkflowsModel
				{
					ID = id,
					RepositoryID = repositoryId,
					Name = workflow.Name,
					State = workflow.State,
					Path = workflow.Path,
					CreatedAt = workflow.CreatedAt,
					UpdatedAt = workflow.UpdatedAt,
					DeletedAt = workflow.DeletedAt,
					LastPolledAt = utcNow,
				};

				rows.Add(WorkflowsRowConverter.Instance.ToRawValue(row));
			}

			if (rows.Count > 0)
			{
				SLTables.Workflows.FillTableNoDelete(protocol, rows);
			}

			HandleNextRepositoryWorkflowPage(protocol, owner, name);
		}

		public static void HandleExecuteWorkflowResponse(SLProtocol protocol)
		{
			// Check status code
			var message = $"Successfully executed the given workflow.";
			if (!protocol.IsSuccessStatusCode())
			{
				var errorResponse = Convert.ToString(protocol.GetParameter(Parameter.postexecuteworkflowcontent_231));
				var error = SecureNewtonsoftDeserialization.DeserializeObject<GithubError>(errorResponse);
				message = $"Received error code {error.Status}: {error.Message}";
			}

			// Parse response
			var url = Convert.ToString(protocol.GetParameter(Parameter.postworkflowexecutionurl_131));

			// Parse url to check which repository this workflow dispatch is linked to
			var match = WorkflowDispatchUrlRegex.Match(url);
			var owner = match.Groups[1].Value;
			var name = match.Groups[2].Value;
			var workflowId = HttpUtility.UrlDecode(match.Groups[3].Value);

			HandleWorkflowExecutionInterApp(protocol, owner, name, workflowId, message);
		}

		public static void HandleWorkflowExecutionInterApp(SLProtocol protocol, string owner, string name, string workflowId, string message)
		{
			// Check if there are Topics InterApp messages waiting for confirmation
			var table = IAC_MessagesTable.GetTable(protocol);

			foreach (var iacRow in table.Rows.Where(iac => iac.ResponseType.AssemblyQualifiedName == typeof(ExecuteWorkflowResponse).AssemblyQualifiedName))
			{
				var request = (GenericInterAppMessage<ExecuteWorkflowRequest>)iacRow.Request;

				if (request.Data.RepositoryId.Owner == owner &&
					request.Data.RepositoryId.Name == name &&
					request.Data.WorkflowId == workflowId &&
					iacRow.Status == IAC_MessageStatus.InProgress)
				{
					var returnMessage = (GenericInterAppMessage<ExecuteWorkflowResponse>)iacRow.Response;
					returnMessage.Data.Success = true;
					returnMessage.Data.Description = message;
					iacRow.Request.Reply(protocol.SLNet.RawConnection, returnMessage, Types.KnownTypes);
					iacRow.Status = IAC_MessageStatus.Confirmed;
					iacRow.SaveToProtocol(protocol);
				}
			}
		}

		private static void HandleNextRepositoryWorkflowPage(SLProtocol protocol, string owner, string name)
		{
			// Check if there are more workflows to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getrepositoryworkflowslinkheader));
			var link = new LinkHeader(linkHeader);

			// Check if there are more workflows to fetch for the current repository
			if (!string.IsNullOrEmpty(linkHeader))
			{
				if (link.HasNext)
				{
					RepositoriesRequestHandler.HandleRepositoriesTagsRequest(protocol, $"{owner}/{name}", link.NextPage, true);
					return;
				}
				else
				{
					SLTables.Workflows.Cleanup(protocol, $"{owner}/{name}");
				}
			}

			// If no more workflows for this repo fetch the next repository in the queue.
			HandleNextRepositoryWorkflow(protocol);
		}

		private static void HandleNextRepositoryWorkflow(SLProtocol protocol)
		{
			var queue = SecureNewtonsoftDeserialization.DeserializeObject<List<string>>(
				Convert.ToString(protocol.GetParameter(Parameter.getrepositoryworkflowsqueue)));
			var next = queue?.FirstOrDefault();

			if (next == null)
			{
				return;
			}

			protocol.SetParameter(Parameter.getrepositoryworkflowsqueue, JsonConvert.SerializeObject(queue.Skip(1)));
			RepositoriesRequestHandler.HandleRepositoriesWorkflowsRequest(protocol, next, 1, true);
		}
	}
}
