// Ignore Spelling: App Workflows Nuget github

namespace Skyline.Protocol.InterApp.Executors.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
    using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
    using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
    using Skyline.DataMiner.Scripting;
    using Skyline.Protocol.API.Workflows;
    using Skyline.Protocol.PollManager.RequestHandler.Repositories;
    using Skyline.Protocol.Tables;

    public class ExecuteWorkflowExecutor : MessageExecutor<GenericInterAppMessage<ExecuteWorkflowRequest>>
    {
        private ExecuteWorkflowResponse result;

        private RepositoriesTableRow repo;

        public ExecuteWorkflowExecutor(GenericInterAppMessage<ExecuteWorkflowRequest> message) : base(message)
        {
            result = new ExecuteWorkflowResponse
            {
                Request = Message.Data,
                Success = false,
                Description = "An unknown error occurred",
            };
        }

        public override void DataGets(object dataSource)
        {
            // Setup
            var protocol = (SLProtocol)dataSource;

            // Fetch the requested repository information
            repo = RepositoriesTableRow.FromPK(protocol, Message.Data.RepositoryId.FullName);
        }

        public override void Parse() { }

        public override bool Validate()
        {
            if (!WorkflowValidation.Validate(Message.Data, repo, out var error))
            {
                result.Success = false;
                result.Description = error;
                return false;
            }

            return true;
        }

        public override void Modify() { }

        public override void DataSets(object dataDestination)
        {
            // Setup
            var protocol = (SLProtocol)dataDestination;
            var url = $"repos/{repo.Owner}/{repo.Name}/actions/workflows/{Message.Data.WorkflowId}/dispatches";

            // Add to the InterApp Queue
            new IAC_MessagesTableRow
            {
                Guid = Guid.Parse(Message.Guid),
                Status = IAC_MessageStatus.InProgress,
                Request = Message,
                RequestType = typeof(ExecuteWorkflowRequest),
                Response = new GenericInterAppMessage<ExecuteWorkflowResponse>(result),
                ResponseType = typeof(ExecuteWorkflowResponse),
                Info = $"Executing {url}",
            }.SaveToProtocol(protocol);

            var jsonBody = new
            {
                @ref = "main",
                inputs = Message.Data.WorkflowInputs,
            };

            Dictionary<int, object> setParams = new Dictionary<int, object>
            {
                { Parameter.postworkflowexecutionurl_131, url },
                { Parameter.postworkflowexecutionbody_181, JsonConvert.SerializeObject(jsonBody) },
            };
            protocol.Log($"QA{protocol.QActionID}|workflow execute bodY: {JsonConvert.SerializeObject(jsonBody)}", LogType.DebugInfo, LogLevel.NoLogging);
            protocol.SetParameters(setParams.Keys.ToArray(), setParams.Values.ToArray());
            protocol.CheckTrigger(231);

            // Return message
            result = null;
        }

        public override Message CreateReturnMessage()
        {
            if (result != null)
            {
                return new GenericInterAppMessage<ExecuteWorkflowResponse>(result);
            }

            return null;
        }
    }
}
