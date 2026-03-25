namespace Skyline.Protocol.Tables
{
	public static class SLTables
	{
		public static readonly PollManagerQActionTable PollManager;

		public static readonly RepositoriesQActionTable Repositories;

		public static readonly TagsQActionTable Tags;

		public static readonly ReleasesQActionTable Releases;

		public static readonly ReleaseAssetsQActionTable ReleaseAssets;

		public static readonly IssuesQActionTable Issues;

		public static readonly WorkflowsQActionTable Workflows;

		public static readonly SoftwareBillOfMaterialsQActionTable SoftwareBillOfMaterials;

		public static readonly SoftwareBillOfMaterialsPackagesQActionTable SoftwareBillOfMaterialsPackages;

		public static readonly SoftwareBillOfMaterialsRelationshipsQActionTable SoftwareBillOfMaterialsRelationships;

		public static readonly OrganizationsQActionTable Organizations;

		public static readonly MembersQActionTable Members;

		public static readonly TeamsQActionTable Teams;

		public static readonly MemberOrganizationLinksQActionTable MemberOrganizationLinks;

#pragma warning disable S3963
		static SLTables()
		{
			// Static constructor ensures fields initialize in a specific order
			PollManager = PollManagerQActionTable.Singleton;
			Repositories = RepositoriesQActionTable.Singleton;
			Tags = TagsQActionTable.Singleton;
			Releases = ReleasesQActionTable.Singleton;
			ReleaseAssets = ReleaseAssetsQActionTable.Singleton;
			Issues = IssuesQActionTable.Singleton;
			Workflows = WorkflowsQActionTable.Singleton;
			SoftwareBillOfMaterials = SoftwareBillOfMaterialsQActionTable.Singleton;
			SoftwareBillOfMaterialsPackages = SoftwareBillOfMaterialsPackagesQActionTable.Singleton;
			SoftwareBillOfMaterialsRelationships = SoftwareBillOfMaterialsRelationshipsQActionTable.Singleton;

			Organizations = OrganizationsQActionTable.Singleton;
			Members = MembersQActionTable.Singleton;
			Teams = TeamsQActionTable.Singleton;
			MemberOrganizationLinks = MemberOrganizationLinksQActionTable.Singleton;
		}
#pragma warning restore S3963
	}
}
