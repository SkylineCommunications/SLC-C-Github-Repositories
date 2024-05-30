// Ignore Spelling: App

namespace Skyline.Protocol.InterApp.Executors.Repositories
{
	using System;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	public class AddRepositoryExecutor : SimpleMessageExecutor<GenericInterAppMessage<AddRepositoryRequest>>
	{
		public AddRepositoryExecutor(GenericInterAppMessage<AddRepositoryRequest> message) : base(message)
		{
		}

		public override bool TryExecute(object dataSource, object dataDestination, out Message optionalReturnMessage)
		{
			// Setup
			var protocol = (SLProtocol)dataSource;

			var returnMessage = new AddRepositoryResponse
			{
				Request = Message.Data,
				RepositoryId = Message.Data.RepositoryId,
			};

			// Validate request
			if(String.IsNullOrWhiteSpace(Message.Data.RepositoryId.Owner) ||
				String.IsNullOrWhiteSpace(Message.Data.RepositoryId.Name))
			{
				returnMessage.Success = false;
				returnMessage.Description = "The Owner and Name of the repository cannot be left empty.";
				optionalReturnMessage = new GenericInterAppMessage<AddRepositoryResponse>(returnMessage);
				return false;
			}

			// Check if it was already added.
			if(RepositoriesTableRow.FromPK(protocol, $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}") != default)
			{
				returnMessage.Success = true;
				returnMessage.Description = "The repository is already added.";
				optionalReturnMessage = new GenericInterAppMessage<AddRepositoryResponse>(returnMessage); ;
				return true;
			}

			// Add Repository
			var row = new RepositoriesTableRow
			{
				FullName = $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}",
				Owner = Message.Data.RepositoryId.Owner,
				Name = Message.Data.RepositoryId.Name,
			};

			row.SaveToProtocol(protocol);

			// Poll the repository
			RepositoriesRequestHandler.HandleRepositoriesPublicKeysRequest(protocol, Message.Data.RepositoryId.Owner, Message.Data.RepositoryId.Name);
			RepositoriesRequestHandler.HandleRepositoriesRequest(protocol, Message.Data.RepositoryId.Owner, Message.Data.RepositoryId.Name);

			// Return message
			returnMessage.Success = true;
			returnMessage.Description = "Successfully added a new tracked repository.";
			optionalReturnMessage = new GenericInterAppMessage<AddRepositoryResponse>(returnMessage); ;
			return true;
		}
	}
}
