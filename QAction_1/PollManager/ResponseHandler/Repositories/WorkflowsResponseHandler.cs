// Ignore Spelling: Workflows

namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Newtonsoft.Json;
    using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
    using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
    using Skyline.Protocol;
    using Skyline.Protocol.API.Headers;
    using Skyline.Protocol.Extensions;
    using Skyline.Protocol.PollManager.RequestHandler.Repositories;
    using Skyline.Protocol.Tables;

    public static class WorkflowResponseHandler
    {
        public static void HandleRepositoriesWorkflowsResponse(SLProtocol protocol)
        {
            // Check status code
            if (!protocol.IsSuccessStatusCode())
            {
                return;
            }

            // Parse response
            var parameters = (object[])protocol.GetParameters(new uint[] { Parameter.getrepositoryworkflowscontent_205, Parameter.getrepositoryworkflowsurl_105 });
            var response = JsonConvert.DeserializeObject<RepositoryWorkflowsResponse>(Convert.ToString(parameters[0]));
            var url = Convert.ToString(parameters[1]);
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

        public static void HandleExecuteWorkflowResponse(SLProtocol protocol)
        {
            // Check status code
            if (!protocol.IsSuccessStatusCode())
            {
                return;
            }

            var executionUrl = Convert.ToString(protocol.GetParameter(Parameter.postworkflowexecutionurl_131));
            var urlParts = executionUrl.Split('/');
            var owner = urlParts[1];
            var repoName = urlParts[2];
            var workflowId = urlParts[urlParts.Length - 2];

            HandleWorkflowExeuctionInterApp(protocol, owner, repoName, workflowId);
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

        public static void HandleWorkflowExeuctionInterApp(SLProtocol protocol, string owner, string name, string workflowId)
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
                    returnMessage.Data.Description = $"Successfully executed the given workflow.";
                    iacRow.Request.Reply(protocol.SLNet.RawConnection, returnMessage, Types.KnownTypes);
                    iacRow.Status = IAC_MessageStatus.Confirmed;
                    iacRow.SaveToProtocol(protocol);
                }
            }
        }
    }
}
