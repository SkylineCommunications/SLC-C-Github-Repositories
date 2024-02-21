// Ignore Spelling: App

namespace Skyline.Protocol.InterApp.Executors.Repositories
{
	using System;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public class AddRepositoryExecutor : SimpleMessageExecutor<AddRepositoryRequest>
	{
		public AddRepositoryExecutor(AddRepositoryRequest message) : base(message)
		{
		}

		public override bool TryExecute(object dataSource, object dataDestination, out Message optionalReturnMessage)
		{
			// Setup
			var protocol = (SLProtocol)dataSource;

			var returnMessage = new AddRepositoryResponse
			{
				Request = Message,
				RepositoryId = Message.RepositoryId,
			};

			// Validate request
			if(String.IsNullOrWhiteSpace(Message.RepositoryId.Owner) ||
				String.IsNullOrWhiteSpace(Message.RepositoryId.Name))
			{
				returnMessage.Success = false;
				returnMessage.Description = "The Owner and Name of the repository cannot be left empty.";
				optionalReturnMessage = returnMessage;
				return false;
			}

			// Check if it was already added.
			if(RepositoriesTableRow.FromPK(protocol, $"{Message.RepositoryId.Owner}/{Message.RepositoryId.Name}") != default)
			{
				returnMessage.Success = true;
				returnMessage.Description = "The repository is already added.";
				optionalReturnMessage = returnMessage;
				return true;
			}

			// Add Repository
			var row = new RepositoriesTableRow
			{
				FullName = $"{Message.RepositoryId.Owner}/{Message.RepositoryId.Name}",
				Owner = Message.RepositoryId.Owner,
				Name = Message.RepositoryId.Name,
			};

			row.SaveToProtocol(protocol);

			// Return message
			returnMessage.Success = true;
			returnMessage.Description = "Successfully added a new tracked repository.";
			optionalReturnMessage = returnMessage;
			return true;
		}
	}
}
