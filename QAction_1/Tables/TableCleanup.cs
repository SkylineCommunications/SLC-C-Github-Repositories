namespace Skyline.Protocol.Tables
{
	using System;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.TableCleanup;
	using Skyline.DataMiner.Utils.TableCleanup.Filters;
	using Skyline.Protocol.PollManager;

	public class TableCleanup
	{
		public static void Cleanup(SLProtocol protocol, bool executeNow)
		{
			CleanupInterApp(protocol);
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Table_Cleanup)));
			if(!pollRow.LastPolledUTCTime.HasValue)
			{
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Table_Cleanup, PollingStatus.Idle);
				return;
			}

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
			SLTables.PollManager.SetPollingStatus(protocol, RequestType.Table_Cleanup, PollingStatus.Idle);
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
