namespace Skyline.Protocol.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.Scripting;
    using Skyline.Protocol.Extensions;
    using SLNetMessages = Skyline.DataMiner.Net.Messages;

    public class RepositoryTagsTableRow
	{
		public RepositoryTagsTableRow() { }

		public RepositoryTagsTableRow(params object[] row)
		{
			ID = Convert.ToString(row[0]);
			Name = Convert.ToString(row[1]);
			RepositoryID = Convert.ToString(row[2]);
			CommitSHA = Convert.ToString(row[3]);
		}

		public string ID { get; set; }

		public string Name { get; set; }

		public string RepositoryID { get; set; }

		public string CommitSHA { get; set; }

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
			};
			object[] repositorytags = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorytags.tablePid, repositoryTagsIdx);
			object[] iD = (object[])repositorytags[0];
			object[] name = (object[])repositorytags[1];
			object[] repositoryID = (object[])repositorytags[2];
			object[] commitSHA = (object[])repositorytags[3];

			for (int i = 0; i < iD.Length; i++)
			{
				Rows.Add(new RepositoryTagsTableRow(
				iD[i],
				name[i],
				repositoryID[i],
				commitSHA[i]));
			}
		}

		~RepositoryTagsTable()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(false);
		}
		#endregion

		public List<RepositoryTagsTableRow> Rows { get; private set; } = new List<RepositoryTagsTableRow>();

		public static RepositoryTagsTable GetTable(SLProtocol protocol = null)
		{
			if (protocol != null)
			{
				instance.Dispose();
				instance = new RepositoryTagsTable(protocol);
			}

			return instance;
		}

		public void SaveToProtocol(SLProtocol protocol, bool partial = false)
		{
			List<object[]> rows = Rows.Select(x => x.ToProtocolRow()).ToList();
			NotifyProtocol.SaveOption option = partial ? NotifyProtocol.SaveOption.Partial : NotifyProtocol.SaveOption.Full;
			protocol.FillArray(Parameter.Repositorytags.tablePid, rows, option);
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

			e.Protocol.DeleteRow(Parameter.Repositorytags.tablePid, tagRows.ToArray());
		}
	}
}
