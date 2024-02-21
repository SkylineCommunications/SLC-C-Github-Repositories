// Ignore Spelling: App

namespace Skyline.Protocol.InterApp.Executors.Repositories
{
	using System;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public class RemoveRepositoryExecutor : SimpleMessageExecutor<RemoveRepositoryRequest>
	{
		public RemoveRepositoryExecutor(RemoveRepositoryRequest message) : base(message)
		{
		}

		public override bool TryExecute(object dataSource, object dataDestination, out Message optionalReturnMessage)
		{
			// Setup
			var protocol = (SLProtocol)dataSource;

			var returnMessage = new RemoveRepositoryResponse
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

			// Check if it was already removed.
			if(RepositoriesTableRow.FromPK(protocol, $"{Message.RepositoryId.Owner}/{Message.RepositoryId.Name}") == default)
			{
				returnMessage.Success = true;
				returnMessage.Description = "The repository is already removed.";
				optionalReturnMessage = returnMessage;
				return true;
			}

			// Remove Repository
			RepositoriesTable.GetTable().DeleteRow(protocol, $"{Message.RepositoryId.Owner}/{Message.RepositoryId.Name}");

			// Return message
			returnMessage.Success = true;
			returnMessage.Description = "Successfully removed the tracked repository.";
			optionalReturnMessage = returnMessage;
			return true;
		}
	}
}
