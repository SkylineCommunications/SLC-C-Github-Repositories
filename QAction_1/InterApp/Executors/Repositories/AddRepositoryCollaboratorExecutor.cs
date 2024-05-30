// Ignore Spelling: App Workflows

namespace Skyline.Protocol.InterApp.Executors.Workflows
{
	using System;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Core.InterAppCalls.Common.MessageExecution;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.PollManager.RequestHandler.Organizations;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	public class AddRepositoryCollaboratorExecutor : MessageExecutor<GenericInterAppMessage<AddRepositoryCollaboratorRequest>>
	{
		private AddRepositoryCollaboratorResponse result;

		private OrganizationsTableRow organization;
		private RepositoriesTableRow repo;
		private TeamsTableRow team;
		private MembersTableRow member;

		public AddRepositoryCollaboratorExecutor(GenericInterAppMessage<AddRepositoryCollaboratorRequest> message) : base(message)
		{
			result = new AddRepositoryCollaboratorResponse
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
			organization = OrganizationsTableRow.FromPK(protocol, Message.Data.Data.RepositoryOrganization);
			repo = RepositoriesTableRow.FromPK(protocol, Message.Data.RepositoryId.FullName);
			team = TeamsTableRow.FromPK(protocol, $"{Message.Data.Data.RepositoryOrganization}/{Message.Data.Data.CollaboratorSlug}");
			member = MembersTableRow.FromPK(protocol, Message.Data.Data.CollaboratorSlug);
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

			// Check given organization if it's a team collaborator
			if (Message.Data.Data.CollaboratorType == CollaboratorType.Team &&
				String.IsNullOrWhiteSpace(Message.Data.Data.RepositoryOrganization))
			{
				result.Success = false;
				result.Description = "The Organization cannot be left empty, when trying to add a team.";
				return false;
			}

			// Check the Slug
			if (String.IsNullOrWhiteSpace(Message.Data.Data.CollaboratorSlug))
			{
				result.Success = false;
				result.Description = "The slug name of the collaborator cannot be left empty.";
				return false;
			}

			// Check if the User Exists on the connector
			if (Message.Data.Data.CollaboratorType == CollaboratorType.User && member == default)
			{
				result.Success = false;
				result.Description = $"The element doesn't have a user named '{Message.Data.Data.CollaboratorSlug}'.";
				return false;
			}

			// Check if the Organization Exists on the connector
			if (Message.Data.Data.CollaboratorType == CollaboratorType.Team && organization == default)
			{
				result.Success = false;
				result.Description = $"The element doesn't have an organization named '{Message.Data.Data.RepositoryOrganization}'.";
				return false;
			}

			// Check if the Team Exists on the connector
			if (Message.Data.Data.CollaboratorType == CollaboratorType.Team && team == default)
			{
				result.Success = false;
				result.Description = $"The element doesn't have a team named '{Message.Data.Data.CollaboratorSlug}'.";
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

			// Create the information for the InterApp Queue
			string info = String.Empty;
			if (Message.Data.Data.CollaboratorType == CollaboratorType.User)
			{
				info = $"{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}/{Message.Data.Data.CollaboratorSlug}";
			}

			if (Message.Data.Data.CollaboratorType == CollaboratorType.Team)
			{
				info = $"{Message.Data.Data.RepositoryOrganization}/{Message.Data.RepositoryId.Owner}/{Message.Data.RepositoryId.Name}/{Message.Data.Data.CollaboratorSlug}";
			}

			// Add to the InterApp Queue
			new IAC_MessagesTableRow
			{
				Guid = Guid.Parse(Message.Guid),
				Status = IAC_MessageStatus.InProgress,
				Request = Message,
				RequestType = typeof(AddRepositoryCollaboratorRequest),
				Response = new GenericInterAppMessage<AddRepositoryCollaboratorResponse>(result),
				ResponseType = typeof(AddRepositoryCollaboratorResponse),
				Info = info,
			}.SaveToProtocol(protocol);

			// Add the user/team to the repository
			if(Message.Data.Data.CollaboratorType == CollaboratorType.User)
			{
				RepositoriesRequestHandler.HandleOrganizationAddRepositoryCollaborator(protocol,
					Message.Data.RepositoryId.Owner,
					Message.Data.RepositoryId.Name,
					Message.Data.Data.CollaboratorSlug,
					Message.Data.Data.Permission);
			}
			else if(Message.Data.Data.CollaboratorType == CollaboratorType.Team)
			{
				OrganizationsRequestHandler.HandleOrganizationAddRepositoryCollaborator(protocol,
					Message.Data.Data.RepositoryOrganization,
					Message.Data.RepositoryId.Owner,
					Message.Data.RepositoryId.Name,
					Message.Data.Data.CollaboratorSlug,
					Message.Data.Data.Permission);
			}
			else
			{
				// Nothing todo yet.
			}

			// Return message
			result = null;
		}

		public override Message CreateReturnMessage()
		{
			if(result != null)
			{
				return new GenericInterAppMessage<AddRepositoryCollaboratorResponse>(result);
			}

			return null;
		}
	}
}