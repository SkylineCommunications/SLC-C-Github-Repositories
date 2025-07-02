// Ignore Spelling: App Workflows

namespace Skyline.Protocol.InterApp.Executors.Workflows
{
	using System;
	using System.IO;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.SecureCoding.SecureIO;
	using Skyline.Protocol.API.Workflows;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	public class CreateRepositoryContentExecutor : MessageExecutor<GenericInterAppMessage<CreateRepositoryContentRequest>>
	{
		private CreateRepositoryContentResponse result;

		private RepositoriesTableRow repo;

		public CreateRepositoryContentExecutor(GenericInterAppMessage<CreateRepositoryContentRequest> message) : base(message)
		{
			result = new CreateRepositoryContentResponse
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

			// Fetch the requested organization information
			repo = RepositoriesTableRow.FromPK(protocol, Message.Data.RepositoryId.FullName);
		}

		public override void Parse() { }

		public override bool Validate()
		{
			// Check given repository id
			if (String.IsNullOrWhiteSpace(Message.Data.RepositoryId.Owner) ||
				String.IsNullOrWhiteSpace(Message.Data.RepositoryId.Name))
			{
				result.Success = false;
				result.Description = "The Owner and Name of the repository cannot be left empty.";
				return false;
			}

			// Check the repository path
			if (String.IsNullOrWhiteSpace(Message.Data.RepositoryPath))
			{
				result.Success = false;
				result.Description = "The Repository Path cannot be left blank. This is needed to know which file to create or update.";
				return false;
			}

			// Check file path
			if (Message.Data.Data.Method	== UpdateMethod.File && String.IsNullOrWhiteSpace(Message.Data.Data.Path))
			{
				result.Success = false;
				result.Description = "If the Update Method is File, then the path cannot be left empty.";
				return false;
			}

			// Check if file exists
			if (Message.Data.Data.Method == UpdateMethod.File && !File.Exists(SecurePath.CreateSecurePath(Message.Data.Data.Path)))
			{
				result.Success = false;
				result.Description = "The given file does not exists on the system.";
				return false;
			}

			// Check if the repository exists in the connector
			if (repo == default)
			{
				result.Success = false;
				result.Description = $"The given repository '{Message.Data.RepositoryId.FullName}', is not tracked by this element";
				return false;
			}

			return true;
		}

		public override void Modify() { }

		public override void DataSets(object dataDestination)
		{
			// Setup
			var protocol = (SLProtocol)dataDestination;

			// Get the content
			string content;
			if(Message.Data.Data.Method == UpdateMethod.Raw)
			{
				content = Message.Data.Data.Content;
			}
			else
			{
				content = File.ReadAllText(SecurePath.CreateSecurePath(Message.Data.Data.Path));
			}

			var commitMessage = $"Updating '{Message.Data.RepositoryPath}'";
			if(!String.IsNullOrEmpty(Message.Data.CommitMessage))
			{
				commitMessage = Message.Data.CommitMessage;
			}

			// Add to the InterApp Queue
			new IAC_MessagesTableRow
			{
				Guid = Guid.Parse(Message.Guid),
				Status = IAC_MessageStatus.InProgress,
				Request = Message,
				RequestType = typeof(CreateRepositoryContentRequest),
				Response = new GenericInterAppMessage<CreateRepositoryContentResponse>(result),
				ResponseType = typeof(CreateRepositoryContentResponse),
				Info = Message.Data.RepositoryPath,
			}.SaveToProtocol(protocol);

			// Do the actual commit to the repository
			RepositoriesRequestHandler.CreateRepositoryContent(protocol, Message.Data.RepositoryId.FullName, Message.Data.RepositoryPath, content, commitMessage);

			// Return message
			result = null;
		}

		public override Message CreateReturnMessage()
		{
			if(result != null)
			{
				return new GenericInterAppMessage<CreateRepositoryContentResponse>(result);
			}

			return null;
		}
	}
}
