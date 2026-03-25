// Ignore Spelling: App

namespace Skyline.Protocol.InterApp.Executors.Repositories
{
	using System;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public class RemoveRepositoryExecutor : SimpleMessageExecutor<GenericInterAppMessage<RemoveRepositoryRequest>>
	{
		public RemoveRepositoryExecutor(GenericInterAppMessage<RemoveRepositoryRequest> message) : base(message)
		{
		}

		public override bool TryExecute(object dataSource, object dataDestination, out Message optionalReturnMessage)
		{
			// Setup
			var protocol = (SLProtocol)dataSource;

			var returnMessage = new RemoveRepositoryResponse
			{
				Request = Message.Data,
				RepositoryId = Message.Data.RepositoryId,
			};

			// Validate request
			if (String.IsNullOrWhiteSpace(Message.Data.RepositoryId.Owner) ||
				String.IsNullOrWhiteSpace(Message.Data.RepositoryId.Name))
			{
				returnMessage.Success = false;
				returnMessage.Description = "The Owner and Name of the repository cannot be left empty.";
				optionalReturnMessage = new GenericInterAppMessage<RemoveRepositoryResponse>(returnMessage);

				// Add to the InterApp Queue
				new IAC_MessagesTableRow
				{
					Guid = Guid.Parse(Message.Guid),
					Status = IAC_MessageStatus.Confirmed,
					Request = Message,
					RequestType = typeof(RemoveRepositoryRequest),
					Response = optionalReturnMessage,
					ResponseType = typeof(RemoveRepositoryResponse),
					Info = $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}",
					ReceivedAt = DateTime.Now,
				}.SaveToProtocol(protocol);

				return false;
			}

			// Check if it was already removed.
			if (!SLTables.Repositories.TryGetRow(protocol, $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}", out var rawRow))
			{
				returnMessage.Success = true;
				returnMessage.Description = "The repository is already removed.";
				optionalReturnMessage = new GenericInterAppMessage<RemoveRepositoryResponse>(returnMessage);

				// Add to the InterApp Queue
				new IAC_MessagesTableRow
				{
					Guid = Guid.Parse(Message.Guid),
					Status = IAC_MessageStatus.Confirmed,
					Request = Message,
					RequestType = typeof(RemoveRepositoryRequest),
					Response = optionalReturnMessage,
					ResponseType = typeof(RemoveRepositoryResponse),
					Info = $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}",
					ReceivedAt = DateTime.Now,
				}.SaveToProtocol(protocol);

				return true;
			}

			var row = RepositoriesRowConverter.Instance.FromRawValue(rawRow);
			var isOrganizationTracked = SLTables.Organizations.Tracked.Read.GetCell(protocol, row.Owner).Value;
			if (isOrganizationTracked.HasValue && isOrganizationTracked.Value)
			{
				// If the organization is tracked we cannot remove it.
				SLTables.Repositories.AutoRemove.Read.SetCell(protocol, row.FullName, true);

				returnMessage.Success = false;
				returnMessage.Description = "Cannot remove the repository as it's owner organization is tracked. Enabled Auto-Remove instead.";
				optionalReturnMessage = new GenericInterAppMessage<RemoveRepositoryResponse>(returnMessage);

				// Add to the InterApp Queue
				new IAC_MessagesTableRow
				{
					Guid = Guid.Parse(Message.Guid),
					Status = IAC_MessageStatus.Confirmed,
					Request = Message,
					RequestType = typeof(RemoveRepositoryRequest),
					Response = optionalReturnMessage,
					ResponseType = typeof(RemoveRepositoryResponse),
					Info = $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}",
					ReceivedAt = DateTime.Now,
				}.SaveToProtocol(protocol);

				return true;
			}

			// Remove Repository
			SLTables.Repositories.DeleteRow(protocol, $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}");

			// Return message
			returnMessage.Success = true;
			returnMessage.Description = "Successfully removed the tracked repository.";
			optionalReturnMessage = new GenericInterAppMessage<RemoveRepositoryResponse>(returnMessage);

			// Add to the InterApp Queue
			new IAC_MessagesTableRow
			{
				Guid = Guid.Parse(Message.Guid),
				Status = IAC_MessageStatus.Confirmed,
				Request = Message,
				RequestType = typeof(RemoveRepositoryRequest),
				Response = optionalReturnMessage,
				ResponseType = typeof(RemoveRepositoryResponse),
				Info = $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}",
				ReceivedAt = DateTime.Now,
			}.SaveToProtocol(protocol);

			return true;
		}
	}
}