// Ignore Spelling: App Workflows

namespace Skyline.Protocol.InterApp.Executors.Workflows
{
	using System;
	using System.Text.RegularExpressions;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Organizations;
	using Skyline.Protocol.PollManager.RequestHandler.Organizations;
	using Skyline.Protocol.Tables;

	public class CreateRepositoryExecutor : MessageExecutor<GenericInterAppMessage<CreateRepositoryRequest>>
	{
		private CreateRepositoryResponse result;

		private OrganizationsModel organization;

		public CreateRepositoryExecutor(GenericInterAppMessage<CreateRepositoryRequest> message) : base(message)
		{
			result = new CreateRepositoryResponse
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
			if (SLTables.Organizations.TryGetRow(protocol, Message.Data.Data.OrganizationId, out var rawOrg))
			{
				organization = OrganizationsRowConverter.Instance.FromRawValue(rawOrg);
			}
		}

		public override void Parse() { }

		public override bool Validate()
		{
			// Check given repository id
			if (String.IsNullOrWhiteSpace(Message.Data.Data.OrganizationId))
			{
				result.Success = false;
				result.Description = "The Owner/Organization cannot be left empty.";
				return false;
			}

			// Check sonarcloud project id
			if (String.IsNullOrWhiteSpace(Message.Data.Data.Name))
			{
				result.Success = false;
				result.Description = "The name cannot be left empty.";
				return false;
			}

			// Check if the name is not too long
			if (Message.Data.Data.Name.Length > 100)
			{
				result.Success = false;
				result.Description = "The name is too long. It should be between 1 and 100 characters.";
				return false;
			}

			// Check if the doesn't contain invalid characters
			var pattern = @"^[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*$";
			if (!Regex.IsMatch(Message.Data.Data.Name, pattern))
			{
				result.Success = false;
				result.Description = "The name contains invalid characters. It can only contain alphanumeric characters or hyphens (-) and it cannot start or end with a hyphen.";
				return false;
			}

			// Check if the organization exists in the connector
			if (organization == default)
			{
				result.Success = false;
				result.Description = $"The given organization '{Message.Data.Data.OrganizationId}', is not tracked by this element or the provided API Key does not have access to it.";
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
			var repo = new CreateRepository
			{
				Name = Message.Data.Data.Name,
				Description = Message.Data.Data.Description,
				Private = !Message.Data.Data.Public,
				Visibility = Message.Data.Data.Public ? "public" : "private",
			};

			// Add to the InterApp Queue
			new IAC_MessagesTableRow
			{
				Guid = Guid.Parse(Message.Guid),
				Status = IAC_MessageStatus.InProgress,
				Request = Message,
				RequestType = typeof(CreateRepositoryRequest),
				Response = new GenericInterAppMessage<CreateRepositoryResponse>(result),
				ResponseType = typeof(CreateRepositoryResponse),
				Info = $"{Message.Data.Data.OrganizationId}/{Message.Data.Data.Name}",
				ReceivedAt = DateTime.Now,
			}.SaveToProtocol(protocol);

			// Do the actual create of the repository
			OrganizationsRequestHandler.HandleOrganizationCreateRepositoryRequest(protocol, Message.Data.Data.OrganizationId, repo);

			// Return message
			result = null;
		}

		public override Message CreateReturnMessage()
		{
			if (result != null)
			{
				return new GenericInterAppMessage<CreateRepositoryResponse>(result);
			}

			return null;
		}
	}
}
