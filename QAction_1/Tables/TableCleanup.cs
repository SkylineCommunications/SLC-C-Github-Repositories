namespace Skyline.Protocol.Tables
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Extensions;

	using SLNetMessages = Skyline.DataMiner.Net.Messages;

	public class TableCleanup
	{
		public static void Cleanup(SLProtocol protocol)
		{
			var repos = RepositoriesTable.GetTable(protocol).Rows.Select(row => row.FullName).ToHashSet();
			CleanupTags(protocol, repos);
			CleanupReleases(protocol, repos);
			CleanupReleasesAssets(protocol, repos);
			CleanupIssues(protocol, repos);
			CleanupWorkflows(protocol, repos);
		}

		private static void CleanupTags(SLProtocol protocol, HashSet<string> repositoryIds)
		{
			// Delete Linked Tags
			var workflowsIdx = new uint[]
			{
				Parameter.Repositorytags.Idx.repositorytagsid_1201,
				Parameter.Repositorytags.Idx.repositorytagsrepositoryid_1203,
			};

			var toBeRemoved = ((object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorytags.tablePid, workflowsIdx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => !repositoryIds.Contains(row[1]))
				.Select(row => row[0]);

			RepositoryTagsTable.GetTable().DeleteRow(protocol, toBeRemoved.ToArray());
		}

		private static void CleanupReleases(SLProtocol protocol, HashSet<string> repositoryIds)
		{
			// Delete Linked Releases
			var workflowsIdx = new uint[]
			{
				Parameter.Repositoryreleases.Idx.repositoryreleasesinstance_1401,
				Parameter.Repositoryreleases.Idx.repositoryreleasesrepositoryid_1413,
			};

			var toBeRemoved = ((object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryreleases.tablePid, workflowsIdx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => !repositoryIds.Contains(row[1]))
				.Select(row => row[0]);

			RepositoryReleasesTable.GetTable().DeleteRow(protocol, toBeRemoved.ToArray());
		}

		private static void CleanupReleasesAssets(SLProtocol protocol, HashSet<string> repositoryIds)
		{
			// Delete Linked Release Assets
			var workflowsIdx = new uint[]
			{
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsinstance_1801,
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsrepositoryid_1803,
			};

			var toBeRemoved = ((object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryreleaseassets.tablePid, workflowsIdx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => !repositoryIds.Contains(row[1]))
				.Select(row => row[0]);

			RepositoryWorkflowsTable.GetTable().DeleteRow(protocol, toBeRemoved.ToArray());
		}

		private static void CleanupIssues(SLProtocol protocol, HashSet<string> repositoryIds)
		{
			// Delete Linked Release Assets
			var workflowsIdx = new uint[]
			{
				Parameter.Repositoryissues.Idx.repositoryissuesinstance_2001,
				Parameter.Repositoryissues.Idx.repositoryissuesrepositoryid_2011,
			};

			var toBeRemoved = ((object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryissues.tablePid, workflowsIdx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => !repositoryIds.Contains(row[1]))
				.Select(row => row[0]);

			RepositoryIssuesTable.GetTable().DeleteRow(protocol, toBeRemoved.ToArray());
		}

		private static void CleanupWorkflows(SLProtocol protocol, HashSet<string> repositoryIds)
		{
			// Delete Linked Release Assets
			var workflowsIdx = new uint[]
			{
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsid_1601,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsrepositoryid_1602,
			};

			var toBeRemoved = ((object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryworkflows.tablePid, workflowsIdx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => !repositoryIds.Contains(row[1]))
				.Select(row => row[0]);

			RepositoryWorkflowsTable.GetTable().DeleteRow(protocol, toBeRemoved.ToArray());
		}
	}
}
