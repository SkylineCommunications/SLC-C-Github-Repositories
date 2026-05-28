namespace Skyline.Protocol.PollManager
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	public enum Triggers
	{
		GetRepository = 201,
		GetRepositoryIssues = 202,
		GetRepositoryTags = 203,
		GetRepositoryReleases = 204,
		GetRepositoryWorkflows = 205,
		GetRepositorySoftwareBillOfMaterials = 206,
		GetUserOrganizations = 210,
		GetOrganizationRepositories = 211,
		GetOrganizationTeams = 212,
		GetOrganizationMembers = 213,
		GetRepositoryContent = 220,
		PutRepositoryContent = 221,
		PostRepository = 222,
		PutRepositoryUserCollaborator = 223,
		PutRepositoryTeamCollaborator = 224,
		PutRepositorySecret = 226,
		GetRepositoryPublicKey = 228,
		GetRepositoryTopics = 229,
		PutRepositoryTopics = 230,
		PostWorkflowExecution = 231,
		PostRepositoryVariable = 235,

		GetRepositoryNow = 401,
		GetRepositoryIssuesNow = 402,
		GetRepositoryTagsNow = 403,
		GetRepositoryReleasesNow = 404,
		GetRepositoryWorkflowsNow = 405,
		GetRepositorySoftwareBillOfMaterialsNow = 406,
		GetUserOrganizationsNow = 410,
		GetOrganizationRepositoriesNow = 411,
		GetOrganizationTeamsNow = 412,
		GetOrganizationMembersNow = 413,
		GetRepositoryContentNow = 420,
		PutRepositoryContentNow = 421,
		PostRepositoryNow = 422,
		PutRepositoryUserCollaboratorNow = 423,
		PutRepositoryTeamCollaboratorNow = 424,
		PutRepositorySecretNow = 426,
		GetRepositoryPublicKeyNow = 428,
		GetRepositoryTopicsNow = 429,
		PutRepositoryTopicsNow = 430,
		PostWorkflowExecutionNow = 431,
		PostRepositoryVariableNow = 435,
	}
}
