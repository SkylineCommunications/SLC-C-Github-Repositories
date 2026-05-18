// Ignore Spelling: App Workflows Nuget github

namespace Skyline.Protocol.InterApp.Executors.Workflows
{
	using System;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	public class AddCustomWorkflowExecutor : MessageExecutor<GenericInterAppMessage<AddCustomWorkflowRequest>>
	{
		private AddWorkflowResponse result;

		private RepositoriesModel repo;

		public AddCustomWorkflowExecutor(GenericInterAppMessage<AddCustomWorkflowRequest> message) : base(message)
		{
			result = new AddWorkflowResponse
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
			if (SLTables.Repositories.TryGetRow(protocol, Message.Data.RepositoryId.FullName, out var rawRepo))
			{
				repo = RepositoriesRowConverter.Instance.FromRawValue(rawRepo);
			}
		}

		public override void Parse()
		{
		}

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

		public override void Modify()
		{
		}

		public override void DataSets(object dataDestination)
		{
			// Setup
			var protocol = (SLProtocol)dataDestination;

			// Add to the InterApp Queue
			new IAC_MessagesTableRow
			{
				Guid = Guid.Parse(Message.Guid),
				Status = IAC_MessageStatus.InProgress,
				Request = Message,
				RequestType = typeof(AddCustomWorkflowRequest),
				Response = new GenericInterAppMessage<AddWorkflowResponse>(result),
				ResponseType = typeof(AddWorkflowResponse),
				Info = Message.Data.Workflow.WorkflowName,
				ReceivedAt = DateTime.Now,
			}.SaveToProtocol(protocol);

			// Create the required secrets
			foreach (var secret in Message.Data.Workflow.Secrets)
			{
				RepositoriesRequestHandler.CreateRepositorySecret(protocol, Message.Data.RepositoryId.FullName, secret.Key, secret.Value);
			}

			// Create the required variables
			foreach (var variable in Message.Data.Workflow.Variables)
			{
				RepositoriesRequestHandler.CreateRepositoryVariable(protocol, Message.Data.RepositoryId.FullName, variable.Key, variable.Value);
			}

			// Do the actual commit to the repository
			RepositoriesRequestHandler.CreateRepositoryWorkflow(protocol, Message.Data.RepositoryId.FullName, Message.Data.Workflow.WorkflowName, Message.Data.Workflow.WorkflowYaml);

			// Return message
			result = null;
		}

		public override Message CreateReturnMessage()
		{
			if (result != null)
			{
				return new GenericInterAppMessage<AddWorkflowResponse>(result);
			}

			return null;
		}
	}
}
