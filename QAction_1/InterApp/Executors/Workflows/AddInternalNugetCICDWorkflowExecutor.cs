// Ignore Spelling: App Workflows Nuget github

namespace Skyline.Protocol.InterApp.Executors.Workflows
{
	using System;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.API.Workflows;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

#pragma warning disable S101 // Types should be named in PascalCase
	public class AddInternalNugetCICDWorkflowExecutor : MessageExecutor<AddInternalNugetCICDWorkflowRequest>
#pragma warning restore S101 // Types should be named in PascalCase
	{
		private AddWorkflowResponse result;

		public AddInternalNugetCICDWorkflowExecutor(AddInternalNugetCICDWorkflowRequest message) : base(message)
		{
			result = new AddWorkflowResponse
			{
				Request = Message,
				Success = false,
				Description = "An unknown error occurred",
			};
		}

		public override void DataGets(object dataSource) { }

		public override void Parse() { }

		public override bool Validate()
		{
			// Check given repository id
			if (String.IsNullOrWhiteSpace(Message.RepositoryId.Owner) ||
				String.IsNullOrWhiteSpace(Message.RepositoryId.Name))
			{
				result.Success = false;
				result.Description = "The Owner and Name of the repository cannot be left empty.";
				return false;
			}

			// Check sonarcloud project id
			if (String.IsNullOrWhiteSpace(Message.Data.SonarCloudProjectID))
			{
				result.Success = false;
				result.Description = "The sonar cloud project id cannot be left blank. Go to https://sonarcloud.io/ to retrieve the id.";
				return false;
			}

			// Check Nuget API key
			if (String.IsNullOrWhiteSpace(Message.Data.GithubNugetApiKey))
			{
				result.Success = false;
				result.Description = "The Nuget API Key cannot be left blank.";
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
			var workflow = WorkflowFactory.CreateInternalNugetCICDWorkflow(Message.Data.SonarCloudProjectID);

			// Add to the InterApp Queue
			new IAC_MessagesTableRow
			{
				Guid = Guid.Parse(Message.Guid),
				Status = IAC_MessageStatus.InProgress,
				Request = Message,
				RequestType = typeof(AddNugetCICDWorkflowRequest),
				Response = result,
				ResponseType = typeof(AddWorkflowResponse),
				Info = workflow.Name,
			}.SaveToProtocol(protocol);

			// Create the required secrets
			RepositoriesRequestHandler.CreateRepositorySecret(protocol, Message.RepositoryId.FullName, "NUGETAPIKEY_GITHUB", Message.Data.GithubNugetApiKey);

			// Do the actual commit to the repository
			RepositoriesRequestHandler.CreateRepositoryWorkflow(protocol, Message.RepositoryId.FullName, workflow);

			// Return message
			result = null;
		}

		public override Message CreateReturnMessage()
		{
			return result;
		}
	}
}
