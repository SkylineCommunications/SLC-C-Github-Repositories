namespace Skyline.Protocol.Tables
{
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.TableCleanup;
	using Skyline.DataMiner.Utils.TableCleanup.Filters;

	public class TableCleanup
	{
		public static void Cleanup(SLProtocol protocol)
		{
			CleanupInterApp(protocol);
			SLTables.Tags.Cleanup(protocol);
			SLTables.Releases.Cleanup(protocol);
			SLTables.ReleaseAssets.Cleanup(protocol);
			SLTables.Issues.Cleanup(protocol);
			SLTables.Workflows.Cleanup(protocol);
			SLTables.SoftwareBillOfMaterials.Cleanup(protocol);
			SLTables.SoftwareBillOfMaterialsPackages.Cleanup(protocol);
			SLTables.SoftwareBillOfMaterialsRelationships.Cleanup(protocol);
			SLTables.MemberOrganizationLinks.Cleanup(protocol);

			SLTables.Organizations.Cleanup(protocol);
			SLTables.Teams.Cleanup(protocol);
			SLTables.Members.Cleanup(protocol);
		}

		private static void CleanupInterApp(SLProtocol protocol)
		{
			new TableCleaner(
				protocol,
				Parameter.Iac_messages.tablePid,
				Parameter.Iac_messages.Idx.iac_messagesguid_9000101,
				Parameter.Iac_messages.Idx.iac_messagesreceivedat_9000108)
			.WithCondition(new TableMaxRowCondition(
				Parameter.iac_messagescleanupmethod_9000095,
				Parameter.iac_messagesmaximumrowcount_9000093,
				Parameter.iac_messagesmaximumrowage_9000094))
			.Cleanup();
		}
	}
}
