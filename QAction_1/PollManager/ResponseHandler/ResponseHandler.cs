namespace Skyline.Protocol.PollManager.ResponseHandler
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager.ResponseHandler.Organizations;
	using Skyline.Protocol.PollManager.ResponseHandler.Repositories;

	public static class ResponseHandler
	{
		private static IReadOnlyDictionary<RequestType, Action<SLProtocol>> handlers = new Dictionary<RequestType, Action<SLProtocol>>
		{
			{ RequestType.Repositories_Repositories,            RepositoriesResponseHandler.HandleRepositoriesResponse },
			{ RequestType.Repositories_Tags,                    RepositoriesResponseHandler.HandleRepositoriesTagsResponse },
			{ RequestType.Repositories_Releases,                RepositoriesResponseHandler.HandleRepositoriesReleasesResponse },
			{ RequestType.Repository_Issues,                    RepositoriesResponseHandler.HandleRepositoriesIssuesResponse },
			{ RequestType.Repositories_Workflows,               RepositoriesResponseHandler.HandleRepositoriesWorkflowsResponse },

			{ RequestType.Organizations_User,                   OrganizationsResponseHandler.HandleUserOrganizationsResponse },
			{ RequestType.Organizations_Repositories,           OrganizationsResponseHandler.HandleOrganizationRepositoriesResponse },
			{ RequestType.Organizations_Teams,                  OrganizationsResponseHandler.HandleOrganizationTeamsResponse },
			{ RequestType.Organizations_Members,                OrganizationsResponseHandler.HandleOrganizationMembersResponse },

			{ RequestType.Repositories_CreateOrUpdateContent,   RepositoriesResponseHandler.HandleRepositoryContentResponse },
			{ RequestType.Organizations_CreateRepository,       OrganizationsResponseHandler.HandleOrganizationCreateRepositoryResponse },
			{ RequestType.Repositories_AddUserCollaborator,     RepositoriesResponseHandler.HandleRepositoriesAddRepositoryCollaboratorResponse },
			{ RequestType.Organizations_AddTeamCollaborator,    OrganizationsResponseHandler.HandleOrganizationAddRepositoryCollaboratorResponse },

			{ RequestType.Repositories_CreateOrUpdateSecret,    (protocol) => protocol.IsSuccessStatusCode() },
			{ RequestType.Repositories_CreateVariable,			(protocol) => protocol.IsSuccessStatusCode() },

			{ RequestType.Repositories_PublicKey,               RepositoriesResponseHandler.HandleRepositoriesPublicKeysResponse },
			{ RequestType.Repositories_Topics,                  RepositoriesResponseHandler.HandleRepositoriesTopicsResponse },
			{ RequestType.Repositories_CreateOrUpdateTopics,    RepositoriesResponseHandler.HandleRepositoriesCreateOrUpdateTopicsResponse },
			{ RequestType.Repositories_WorkflowExecution,       RepositoriesResponseHandler.HandleExecuteWorkflowResponse },
		};

		public static IReadOnlyDictionary<RequestType, Action<SLProtocol>> Handlers => handlers;
	}
}
