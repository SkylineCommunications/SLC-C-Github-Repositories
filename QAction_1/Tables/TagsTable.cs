namespace Skyline.Protocol.Tables
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager;
	using Skyline.Protocol.Tables.Events;

	using SLNetMessages = Skyline.DataMiner.Net.Messages;

	public class RepositoryTagsTableRow
	{
		private DateTime lastPolledAt;

		public RepositoryTagsTableRow() { }

		public RepositoryTagsTableRow(params object[] row)
		{
			ID = Convert.ToString(row[0]);
			Name = Convert.ToString(row[1]);
			RepositoryID = Convert.ToString(row[2]);
			CommitSHA = Convert.ToString(row[3]);
			LastPolledAt = DateTime.SpecifyKind(DateTime.FromOADate(Convert.ToDouble(row[4])), DateTimeKind.Utc);
		}

		public string ID { get; set; }

		public string Name { get; set; }

		public string RepositoryID { get; set; }

		public string CommitSHA { get; set; }

		public DateTime LastPolledAt
		{
			get
			{
				return lastPolledAt;
			}

			set
			{
				if (value.Kind != DateTimeKind.Utc)
				{
					throw new ArgumentException("LastPolledAt must be in UTC.");
				}

				lastPolledAt = value;
			}
		}

		public static RepositoryTagsTableRow FromPK(SLProtocol protocol, string pk)
		{
			var row = (object[])protocol.GetRow(Parameter.Repositorytags.tablePid, pk);
			if (row[0] == null)
			{
				return default;
			}

			return new RepositoryTagsTableRow(row);
		}

		public object[] ToProtocolRow()
		{
			return new RepositorytagsQActionRow
			{
				Repositorytagsid_1201 = ID,
				Repositorytagsname_1202 = Name,
				Repositorytagsrepositoryid_1203 = RepositoryID,
				Repositorytagscommitsha_1204 = CommitSHA,
				Repositorytagslastpolledatutc_1199 = LastPolledAt,
			};
		}

		public void SaveToProtocol(SLProtocol protocol)
		{
			if (!protocol.Exists(Parameter.Repositorytags.tablePid, ID))
			{
				protocol.AddRow(Parameter.Repositorytags.tablePid, ToProtocolRow());
			}
			else
			{
				protocol.SetRow(Parameter.Repositorytags.tablePid, ID, ToProtocolRow());
			}
		}
	}

	public class RepositoryTagsTable : IDisposable
	{
		private static RepositoryTagsTable instance = new RepositoryTagsTable();

		#region Constructor
		protected RepositoryTagsTable()
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;
		}

		protected RepositoryTagsTable(SLProtocol protocol)
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;

			uint[] repositoryTagsIdx = new uint[]
			{
				Parameter.Repositorytags.Idx.repositorytagsid_1201,
				Parameter.Repositorytags.Idx.repositorytagsname_1202,
				Parameter.Repositorytags.Idx.repositorytagsrepositoryid_1203,
				Parameter.Repositorytags.Idx.repositorytagscommitsha_1204,
				Parameter.Repositorytags.Idx.repositorytagslastpolledatutc_1199,
			};
			object[] repositorytags = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorytags.tablePid, repositoryTagsIdx);
			object[] iD = (object[])repositorytags[0];
			object[] name = (object[])repositorytags[1];
			object[] repositoryID = (object[])repositorytags[2];
			object[] commitSHA = (object[])repositorytags[3];
			object[] lastPolledAt = (object[])repositorytags[4];

			for (int i = 0; i < iD.Length; i++)
			{
				Rows.Add(new RepositoryTagsTableRow(
				iD[i],
				name[i],
				repositoryID[i],
				commitSHA[i],
				lastPolledAt[i]));
			}
		}

		#endregion

		public static event EventHandler<TableEventArgs> TagsChanged;

		public List<RepositoryTagsTableRow> Rows { get; private set; } = new List<RepositoryTagsTableRow>();

		public static RepositoryTagsTable GetTable(SLProtocol protocol = null)
		{
			if (protocol is null)
			{
				return instance;
			}

			instance?.Dispose();
			instance = new RepositoryTagsTable(protocol);
			return instance;
		}

		public void SaveToProtocol(SLProtocol protocol, bool partial = false)
		{
			// Calculate the batch size, recommended 25000 cells max per fill array.
			var batchSize = 25000 / 4;

			// If full then the first batch needs to be a SaveOption.Full.
			var first = !partial;
			if (!Rows.Any() && !partial)
			{
				protocol.ClearAllKeys(Parameter.Repositorytags.tablePid);
				return;
			}

			foreach (var batch in Rows.Select(x => x.ToProtocolRow()).Batch(batchSize))
			{
				if (first)
				{
					protocol.FillArray(Parameter.Repositorytags.tablePid, batch.ToList(), NotifyProtocol.SaveOption.Full);
				}
				else
				{
					protocol.FillArray(Parameter.Repositorytags.tablePid, batch.ToList(), NotifyProtocol.SaveOption.Partial);
				}

				first = false;
			}
		}

		public void DeleteRow(SLProtocol protocol, params string[] rowsToDelete)
		{
			if (rowsToDelete.Length <= 0)
				return;

			// Remove from DateMiner and local instance
			protocol.DeleteRow(Parameter.Repositorytags.tablePid, rowsToDelete);
			instance.Rows.RemoveAll(x => rowsToDelete.ToList().Contains(x.ID));
			TagsChanged?.Invoke(null, new TableEventArgs(protocol, TableChange.Remove, rowsToDelete));
		}

		public void Cleanup(SLProtocol protocol, string repositoryId)
		{
			var pollRow = PollManagerTable.GetTable(protocol).Rows.FirstOrDefault(r => r.RequestType == RequestType.Repositories_Tags);
			var toBeRemoved = Rows.Where(r => r.RepositoryID == repositoryId)
				.Where(r => r.LastPolledAt < pollRow.LastPolledUTCTime)
				.Select(r => r.ID)
				.ToArray();

			DeleteRow(protocol, toBeRemoved);
		}

		#region IDisposable
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			RepositoriesTable.RepositoriesChanged -= RepositoriesTable_RepositoriesChanged;
		}
		#endregion

		private void RepositoriesTable_RepositoriesChanged(object sender, RepositoryEventArgs e)
		{
			// There only needs to happen something when removing a repository
			if (e.Type != RepositoryChange.Remove)
				return;

			// Delete Linked Tags
			var tagsIdx = new uint[]
			{
				Parameter.Repositorytags.Idx.repositorytagsid,
				Parameter.Repositorytags.Idx.repositorytagsrepositoryid,
			};

			var tagRows = ((object[])e.Protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorytags.tablePid, tagsIdx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => e.Repositories.Contains(row[1]))
				.Select(row => row[0]);

			DeleteRow(e.Protocol, tagRows.ToArray());
		}
	}
}
