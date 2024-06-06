namespace Skyline.Protocol.InterApp.Executors.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Organizations;
	using Skyline.Protocol.PollManager.RequestHandler.Organizations;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	public class AddRepositoryTopicsExecutor : MessageExecutor<GenericInterAppMessage<AddRepositoryTopicsRequest>>
	{
		private AddRepositoryTopicsResponse result;

		private RepositoriesTableRow repository;

		public AddRepositoryTopicsExecutor(GenericInterAppMessage<AddRepositoryTopicsRequest> message) : base(message)
		{
			result = new AddRepositoryTopicsResponse
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
			repository = RepositoriesTableRow.FromPK(protocol, Message.Data.RepositoryId.FullName);
		}

		public override void Parse() { }

		public override bool Validate()
		{
			// Check given repository id
			if (String.IsNullOrWhiteSpace(Message.Data.RepositoryId.Owner) ||
				String.IsNullOrWhiteSpace(Message.Data.RepositoryId.Name))
			{
				result.Success = false;
				result.Description = "The Owner/Name cannot be left empty.";
				return false;
			}

			// Check sonarcloud project id
			if (!Message.Data.Topics.Any())
			{
				result.Success = false;
				result.Description = "No topic's to be added.";
				return false;
			}

			// Check if the doesn't contain invalid characters
			var pattern = @"[, A-Z]";
			if (Message.Data.Topics.Any(topic => Regex.IsMatch(topic, pattern)))
			{
				result.Success = false;
				result.Description = "A topic can only be lower case string and cannot contains spaces or comma's.";
				return false;
			}

			// Check if the organization exists in the connector
			if (repository == default)
			{
				result.Success = false;
				result.Description = $"The given repository '{Message.Data.RepositoryId.FullName}', is not tracked by this element or the provided API Key does not have access to it.";
				return false;
			}

			// Check if the topics are already added
			if(Message.Data.Topics.All(topic => repository.Topics.Contains(topic)))
			{
				result.Success = true;
				result.Description = $"All the topics are already added to the repository.";
				return false;
			}

			return true;
		}

		public override void Modify() { }

		public override void DataSets(object dataDestination)
		{
			// Setup
			var protocol = (SLProtocol)dataDestination;

			// Create the repository object
			var buffer = new List<string>();
			buffer.AddRange(repository.Topics);
			buffer.AddRange(Message.Data.Topics);
			var totalTopics = buffer.Distinct().Where(topic => !String.IsNullOrEmpty(topic));

			// Add to the InterApp Queue
			new IAC_MessagesTableRow
			{
				Guid = Guid.Parse(Message.Guid),
				Status = IAC_MessageStatus.InProgress,
				Request = Message,
				RequestType = typeof(AddRepositoryTopicsRequest),
				Response = new GenericInterAppMessage<AddRepositoryTopicsResponse>(result),
				ResponseType = typeof(AddRepositoryTopicsResponse),
				Info = $"{Message.Data.RepositoryId.FullName}/{String.Join(",", totalTopics)}",
			}.SaveToProtocol(protocol);

			// Do the actual create of the repository
			RepositoriesRequestHandler.CreateOrUpdateRepositoriesTopicsRequest(protocol, Message.Data.RepositoryId.Owner, Message.Data.RepositoryId.Name, totalTopics);

			// Return message
			result = null;
		}

		public override Message CreateReturnMessage()
		{
			if (result != null)
			{
				return new GenericInterAppMessage<AddRepositoryTopicsResponse>(result);
			}

			return null;
		}
	}
}
