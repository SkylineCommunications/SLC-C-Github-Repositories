// Ignore Spelling: App Workflows

namespace Skyline.Protocol.InterApp.Executors.Workflows
{
	using System;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.API.Workflows;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	public class AddConnectorCIWorkflowExecutor : MessageExecutor<GenericInterAppMessage<AddConnectorCIWorkflowRequest>>
	{
		private AddWorkflowResponse result;

		private RepositoriesModel repo;

		public AddConnectorCIWorkflowExecutor(GenericInterAppMessage<AddConnectorCIWorkflowRequest> message) : base(message)
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
			if(SLTables.Repositories.TryGetRow(protocol, Message.Data.RepositoryId.FullName, out var rawRepo))
			{
				repo = RepositoriesRowConverter.Instance.FromRawValue(rawRepo);
			}
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

			// Create workflow file
			var workflow = WorkflowFactory.CreateConnectorCIWorkflow(Message.Data.Data.SonarCloudProjectID);

			// Add to the InterApp Queue
			new IAC_MessagesTableRow
			{
				Guid = Guid.Parse(Message.Guid),
				Status = IAC_MessageStatus.InProgress,
				Request = Message,
				RequestType = typeof(AddConnectorCIWorkflowRequest),
				Response = new GenericInterAppMessage<AddWorkflowResponse>(result),
				ResponseType = typeof(AddWorkflowResponse),
				Info = workflow.Name,
				ReceivedAt = DateTime.Now,
			}.SaveToProtocol(protocol);

			// Create the required secrets
			RepositoriesRequestHandler.CreateRepositorySecret(protocol, Message.Data.RepositoryId.FullName, "DATAMINER_DEPLOY_KEY", Message.Data.Data.DataMinerKey);
			if (!String.IsNullOrWhiteSpace(Message.Data.Data.SonarToken))
			{
				RepositoriesRequestHandler.CreateRepositorySecret(protocol, Message.Data.RepositoryId.FullName, "SONAR_TOKEN", Message.Data.Data.SonarToken);
			}

			// Do the actual commit to the repository
			RepositoriesRequestHandler.CreateRepositoryWorkflow(protocol, Message.Data.RepositoryId.FullName, workflow);

			// Return message
			result = null;
		}

		public override Message CreateReturnMessage()
		{
			if(result != null)
			{
				return new GenericInterAppMessage<AddWorkflowResponse>(result);
			}

			return null;
		}
	}
}
