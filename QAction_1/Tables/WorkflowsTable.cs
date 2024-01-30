// Ignore Spelling: Workflows

namespace Skyline.Protocol.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Scripting;
    using Skyline.Protocol.Extensions;
    using SLNetMessages = Skyline.DataMiner.Net.Messages;

    public class RepositoryWorkflowsTableRow
	{
		private DateTime createdAt;
		private double createdAtOA = Exceptions.IntNotAvailable;
		private DateTime updatedAt;
		private double updatedAtOA = Exceptions.IntNotAvailable;
		private DateTime deletedAt;
		private double deletedAtOA = Exceptions.IntNotAvailable;

		public RepositoryWorkflowsTableRow() { }

		public RepositoryWorkflowsTableRow(params object[] row)
		{
			ID = Convert.ToString(row[0]);
			RepositoryID = Convert.ToString(row[1]);
			Name = Convert.ToString(row[2]);
			State = Convert.ToString(row[3]);
			Path = Convert.ToString(row[4]);
			CreatedAt = DateTime.FromOADate(Convert.ToDouble(row[5]));
			UpdatedAt = DateTime.FromOADate(Convert.ToDouble(row[6]));
			DeletedAt = DateTime.FromOADate(Convert.ToDouble(row[7]));
		}

		public string ID { get; set; }

		public string RepositoryID { get; set; }

		public string Name { get; set; }

		public string State { get; set; }

		public string Path { get; set; }

		public DateTime CreatedAt
		{
			get
			{
				return createdAt;
			}

			set
			{
				if (value == default)
				{
					createdAt = value;
					createdAtOA = Exceptions.IntNotAvailable;
				}
				else
				{
					createdAt = value;
					createdAtOA = value.ToOADate();
				}
			}
		}

		public DateTime UpdatedAt
		{
			get
			{
				return updatedAt;
			}

			set
			{
				if (value == default)
				{
					updatedAt = value;
					updatedAtOA = Exceptions.IntNotAvailable;
				}
				else
				{
					updatedAt = value;
					updatedAtOA = value.ToOADate();
				}
			}
		}

		public DateTime DeletedAt
		{
			get
			{
				return deletedAt;
			}

			set
			{
				if (value == default)
				{
					deletedAt = value;
					deletedAtOA = Exceptions.IntNotAvailable;
				}
				else
				{
					deletedAt = value;
					deletedAtOA = value.ToOADate();
				}
			}
		}

		public static RepositoryWorkflowsTableRow FromPK(SLProtocol protocol, string pk)
		{
			var row = (object[])protocol.GetRow(Parameter.Repositoryworkflows.tablePid, pk);
			if (row[0] == null)
			{
				return default;
			}

			return new RepositoryWorkflowsTableRow(row);
		}

		public object[] ToProtocolRow()
		{
			return new RepositoryworkflowsQActionRow
			{
				Repositoryworkflowsid_1601 = ID,
				Repositoryworkflowsrepositoryid_1602 = RepositoryID,
				Repositoryworkflowsname_1603 = Name,
				Repositoryworkflowsstate_1604 = State,
				Repositoryworkflowspath_1605 = Path,
				Repositoryworkflowscreatedat_1606 = createdAtOA,
				Repositoryworkflowsupdatedat_1607 = updatedAtOA,
				Repositoryworkflowsdeletedat_1608 = deletedAtOA,
			};
		}

		public void SaveToProtocol(SLProtocol protocol)
		{
			if (!protocol.Exists(Parameter.Repositoryworkflows.tablePid, ID))
			{
				protocol.AddRow(Parameter.Repositoryworkflows.tablePid, ToProtocolRow());
			}
			else
			{
				protocol.SetRow(Parameter.Repositoryworkflows.tablePid, ID, ToProtocolRow());
			}
		}
	}

	public class RepositoryWorkflowsTable : IDisposable
	{
		private static RepositoryWorkflowsTable instance = new RepositoryWorkflowsTable();

		#region Constructor
		protected RepositoryWorkflowsTable()
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;
		}

		protected RepositoryWorkflowsTable(SLProtocol protocol)
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;

			uint[] repositoryWorkflowsIdx = new uint[]
			{
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsid_1601,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsrepositoryid_1602,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsname_1603,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsstate_1604,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowspath_1605,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowscreatedat_1606,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsupdatedat_1607,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsdeletedat_1608,
			};
			object[] repositoryworkflows = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryworkflows.tablePid, repositoryWorkflowsIdx);
			object[] iD = (object[])repositoryworkflows[0];
			object[] repositoryID = (object[])repositoryworkflows[1];
			object[] name = (object[])repositoryworkflows[2];
			object[] state = (object[])repositoryworkflows[3];
			object[] path = (object[])repositoryworkflows[4];
			object[] createdAt = (object[])repositoryworkflows[5];
			object[] updatedAt = (object[])repositoryworkflows[6];
			object[] deletedAt = (object[])repositoryworkflows[7];

			for (int i = 0; i < iD.Length; i++)
			{
				Rows.Add(new RepositoryWorkflowsTableRow(
				iD[i],
				repositoryID[i],
				name[i],
				state[i],
				path[i],
				createdAt[i],
				updatedAt[i],
				deletedAt[i]));
			}
		}

		~RepositoryWorkflowsTable()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(false);
		}
		#endregion

		#region Events
		public static event EventHandler<RepositoryWorkflowsEventArgs> WorkflowChanged;
		#endregion

		public List<RepositoryWorkflowsTableRow> Rows { get; private set; } = new List<RepositoryWorkflowsTableRow>();

		public static RepositoryWorkflowsTable GetTable(SLProtocol protocol = null)
		{
			if (protocol != null)
			{
				instance.Dispose();
				instance = new RepositoryWorkflowsTable(protocol);
			}

			return instance;
		}

		public void DeleteRow(SLProtocol protocol, params string[] rowsToDelete)
		{
			if (rowsToDelete.Length <= 0)
				return;

			// Remove from DateMiner and local instance
			protocol.DeleteRow(Parameter.Repositoryworkflows.tablePid, rowsToDelete);
			instance.Rows.RemoveAll(x => rowsToDelete.ToList().Contains(x.ID));

			// Notify other parts that rely on this table.
			var workflowPerRepo = rowsToDelete.Select(x => new
			{
				WorkflowID = x,
				RepositoryID = String.Join("/", x.Split('/')[0], x.Split('/')[1]),
			}).GroupBy(x => x.RepositoryID);

			foreach(var group in workflowPerRepo)
			{
				var repo = group.Key;
				var repoWorkflowsToRemove = group.Select(x => x.WorkflowID).ToArray();
				WorkflowChanged?.Invoke(instance, new RepositoryWorkflowsEventArgs(protocol, WorkflowChange.Remove, repo, repoWorkflowsToRemove));
			}
		}

		public void SaveToProtocol(SLProtocol protocol, bool partial = false)
		{
			List<object[]> rows = Rows.Select(x => x.ToProtocolRow()).ToList();
			NotifyProtocol.SaveOption option = partial ? NotifyProtocol.SaveOption.Partial : NotifyProtocol.SaveOption.Full;
			protocol.FillArray(Parameter.Repositoryworkflows.tablePid, rows, option);
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

		private static void RepositoriesTable_RepositoriesChanged(object sender, RepositoryEventArgs e)
		{
			// There only needs to happen something when removing a repository
			if (e.Type != RepositoryChange.Remove)
				return;

			// Delete Linked Workflows
			var workflowsIdx = new uint[]
			{
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsid,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsrepositoryid,
			};

			var workflowsRows = ((object[])e.Protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryworkflows.tablePid, workflowsIdx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => e.Repositories.Contains(row[1]))
				.Select(row => row[0]);

			e.Protocol.DeleteRow(Parameter.Repositoryworkflows.tablePid, workflowsRows.ToArray());
		}
	}
}
