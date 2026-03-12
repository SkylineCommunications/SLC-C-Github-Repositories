namespace Skyline.Protocol.PollManager.RequestHandler
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.PollManager.RequestHandler.Organizations;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	public static class RequestHandler
	{
		private static IReadOnlyDictionary<RequestType, Action<SLProtocol>> handlers = new Dictionary<RequestType, Action<SLProtocol>>
		{
			{ RequestType.Table_Cleanup,						TableCleanup.Cleanup },

			{ RequestType.Repositories_Repositories,			RepositoriesRequestHandler.HandleRepositoriesRequest },
			{ RequestType.Repositories_Tags,					RepositoriesRequestHandler.HandleRepositoriesTagsRequest },
			{ RequestType.Repositories_Releases,				RepositoriesRequestHandler.HandleRepositoriesReleasesRequest },
			{ RequestType.Repository_Issues,					RepositoriesRequestHandler.HandleRepositoriesIssuesRequest },
			{ RequestType.Repositories_Workflows,				RepositoriesRequestHandler.HandleRepositoriesWorkflowsRequest },
			{ RequestType.Repositories_SoftwareBillOfMaterials, RepositoriesRequestHandler.HandleSoftwareBillOfMaterialsRequest },

			{ RequestType.Organizations_User,					OrganizationsRequestHandler.HandleUserOrganizationsRequest },
			{ RequestType.Organizations_Repositories,			OrganizationsRequestHandler.HandleOrganizationRepositoriesRequest },
			{ RequestType.Organizations_Teams,					OrganizationsRequestHandler.HandleOrganizationsTeamsRequest },
			{ RequestType.Organizations_Members,				OrganizationsRequestHandler.HandleOrganizationsMembersRequest },

			{ RequestType.Repositories_PublicKey,				RepositoriesRequestHandler.HandleRepositoriesPublicKeysRequest },
		};

		public static IReadOnlyDictionary<RequestType, Action<SLProtocol>> Handlers => handlers;
	}
}
