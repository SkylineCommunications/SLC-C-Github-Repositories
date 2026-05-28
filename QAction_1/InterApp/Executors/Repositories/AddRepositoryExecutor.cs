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
			if (String.IsNullOrWhiteSpace(Message.Data.RepositoryId.Owner) ||
				String.IsNullOrWhiteSpace(Message.Data.RepositoryId.Name))
			{
				returnMessage.Success = false;
				returnMessage.Description = "The Owner and Name of the repository cannot be left empty.";
				optionalReturnMessage = new GenericInterAppMessage<AddRepositoryResponse>(returnMessage);

				// Add to the InterApp Queue
				new IAC_MessagesTableRow
				{
					Guid = Guid.Parse(Message.Guid),
					Status = IAC_MessageStatus.Confirmed,
					Request = Message,
					RequestType = typeof(AddRepositoryRequest),
					Response = optionalReturnMessage,
					ResponseType = typeof(AddRepositoryResponse),
					Info = $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}",
					ReceivedAt = DateTime.Now,
				}.SaveToProtocol(protocol);

				return false;
			}

			// Check if it was already added.
			if (SLTables.Repositories.TryGetRow(protocol, $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}", out var rawRow))
			{
				returnMessage.Success = true;
				returnMessage.Description = "The repository is already added.";
				optionalReturnMessage = new GenericInterAppMessage<AddRepositoryResponse>(returnMessage);

				// Add to the InterApp Queue
				new IAC_MessagesTableRow
				{
					Guid = Guid.Parse(Message.Guid),
					Status = IAC_MessageStatus.Confirmed,
					Request = Message,
					RequestType = typeof(AddRepositoryRequest),
					Response = optionalReturnMessage,
					ResponseType = typeof(AddRepositoryResponse),
					Info = $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}",
					ReceivedAt = DateTime.Now,
				}.SaveToProtocol(protocol);

				// Disable the auto-remove for the repository, in case it was previously added with the auto-remove option.
				var existingRow = RepositoriesRowConverter.Instance.FromRawValue(rawRow);
				if (!existingRow.AutoRemove.HasValue || existingRow.AutoRemove.Value)
				{
					SLTables.Repositories.AutoRemove.Read.SetCell(protocol, existingRow.FullName, false);
				}

				return true;
			}

			// Add Repository
			var row = new RepositoriesModel
			{
				FullName = $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}",
				Owner = Message.Data.RepositoryId.Owner,
				Name = Message.Data.RepositoryId.Name,
				AutoRemove = false,
			};

			if (!SLTables.Repositories.TryAddRow(protocol, RepositoriesRowConverter.Instance.ToRawValue(row)))
			{
				returnMessage.Success = true;
				returnMessage.Description = "Could not add the repository to the connector. See the element logging for more details.";
				optionalReturnMessage = new GenericInterAppMessage<AddRepositoryResponse>(returnMessage);

				// Add to the InterApp Queue
				new IAC_MessagesTableRow
				{
					Guid = Guid.Parse(Message.Guid),
					Status = IAC_MessageStatus.Confirmed,
					Request = Message,
					RequestType = typeof(AddRepositoryRequest),
					Response = optionalReturnMessage,
					ResponseType = typeof(AddRepositoryResponse),
					Info = $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}",
					ReceivedAt = DateTime.Now,
				}.SaveToProtocol(protocol);

				return false;
			}

			// Poll the repository
			RepositoriesRequestHandler.HandleRepositoriesPublicKeysRequest(protocol, $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}", true);
			RepositoriesRequestHandler.HandleRepositoriesRequest(protocol, $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}", true);

			// Return message
			returnMessage.Success = true;
			returnMessage.Description = "Successfully added a new tracked repository.";
			optionalReturnMessage = new GenericInterAppMessage<AddRepositoryResponse>(returnMessage);

			// Add to the InterApp Queue
			new IAC_MessagesTableRow
			{
				Guid = Guid.Parse(Message.Guid),
				Status = IAC_MessageStatus.Confirmed,
				Request = Message,
				RequestType = typeof(AddRepositoryRequest),
				Response = optionalReturnMessage,
				ResponseType = typeof(AddRepositoryResponse),
				Info = $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}",
				ReceivedAt = DateTime.Now,
			}.SaveToProtocol(protocol);

			return true;
		}
	}
}