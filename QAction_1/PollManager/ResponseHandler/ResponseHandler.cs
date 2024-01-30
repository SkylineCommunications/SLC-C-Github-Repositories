namespace Skyline.Protocol.PollManager.ResponseHandler
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.PollManager.ResponseHandler.Organizations;
	using Skyline.Protocol.PollManager.ResponseHandler.Repositories;

	public static class ResponseHandler
	{
		private static IReadOnlyDictionary<RequestType, Action<SLProtocol>> handlers = new Dictionary<RequestType, Action<SLProtocol>>
		{
			{ RequestType.Repositories_Repositories,        RepositoriesResponseHandler.HandleRepositoriesResponse },
			{ RequestType.Repositories_Tags,                RepositoriesResponseHandler.HandleRepositoriesTagsResponse },
			{ RequestType.Repositories_Releases,            RepositoriesResponseHandler.HandleRepositoriesReleasesResponse },
			{ RequestType.Repository_Issues,                RepositoriesResponseHandler.HandleRepositoriesIssuesResponse },
			{ RequestType.Repositories_Workflows,           RepositoriesResponseHandler.HandleRepositoriesWorkflowsResponse },

			{ RequestType.Organizations_User,               OrganizationsResponseHandler.HandleUserOrganizationsResponse },
			{ RequestType.Organizations_Repositories,       OrganizationsResponseHandler.HandleOrganizationRepositoriesResponse },

			{ RequestType.Repositories_PublicKey,           RepositoriesResponseHandler.HandleRepositoriesPublicKeysResponse },
		};

		public static IReadOnlyDictionary<RequestType, Action<SLProtocol>> Handlers => handlers;
	}
}
