// Ignore Spelling: Workflows

namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.DataMiner.Scripting.Helper.Events;
	using Skyline.Protocol.PollManager;

	public class WorkflowsRowConverter : ISLRowConverter<WorkflowsModel, RepositoryworkflowsQActionRow>
	{
		public static readonly WorkflowsRowConverter Instance = new WorkflowsRowConverter();

		private readonly ColumnMapBase<WorkflowsModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<WorkflowsModel>[] _columnWriteMaps;

		protected WorkflowsRowConverter()
		{
			_columnMaps = new ColumnMapBase<WorkflowsModel>[]
			{
				SLTables.Workflows.ID.Read.Map<WorkflowsModel>(m => m.ID),
				SLTables.Workflows.RepositoryID.Read.Map<WorkflowsModel>(m => m.RepositoryID),
				SLTables.Workflows.Name.Read.Map<WorkflowsModel>(m => m.Name),
				SLTables.Workflows.State.Read.Map<WorkflowsModel>(m => m.State),
				SLTables.Workflows.Path.Read.Map<WorkflowsModel>(m => m.Path),
				SLTables.Workflows.CreatedAt.Read.Map<WorkflowsModel>(m => m.CreatedAt),
				SLTables.Workflows.UpdatedAt.Read.Map<WorkflowsModel>(m => m.UpdatedAt ),
				SLTables.Workflows.DeletedAt.Read.Map<WorkflowsModel>(m => m.DeletedAt),
				SLTables.Workflows.LastPolledAt.Read.Map<WorkflowsModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<WorkflowsModel>[]
			{
				SLTables.Workflows.ID.Read.MapWrite<WorkflowsModel>(m => m.ID),
				SLTables.Workflows.RepositoryID.Read.MapWrite<WorkflowsModel>(m => m.RepositoryID),
				SLTables.Workflows.Name.Read.MapWrite<WorkflowsModel>(m => m.Name),
				SLTables.Workflows.State.Read.MapWrite<WorkflowsModel>(m => m.State),
				SLTables.Workflows.Path.Read.MapWrite<WorkflowsModel>(m => m.Path),
				SLTables.Workflows.CreatedAt.Read.MapWrite<WorkflowsModel>(m => m.CreatedAt),
				SLTables.Workflows.UpdatedAt.Read.MapWrite<WorkflowsModel>(m => m.UpdatedAt ),
				SLTables.Workflows.DeletedAt.Read.MapWrite<WorkflowsModel>(m => m.DeletedAt),
				SLTables.Workflows.LastPolledAt.Read.MapWrite<WorkflowsModel>(m => m.LastPolledAt),
			};
		}

		public WorkflowsModel FromRawValue(RepositoryworkflowsQActionRow rawRow)
		{
			var row = new WorkflowsModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public RepositoryworkflowsQActionRow ToRawValue(WorkflowsModel row)
		{
			var qactionRow = new RepositoryworkflowsQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new RepositoryworkflowsQActionRow(rawRow);
		}
	}

	public class WorkflowsModel
	{
		public string ID { get; set; }

		public string RepositoryID { get; set; }

		public string Name { get; set; }

		public string State { get; set; }

		public string Path { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public DateTime? DeletedAt { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class WorkflowsQActionTable : SLTable<RepositoryworkflowsQActionRow>
	{
		public static readonly WorkflowsQActionTable Singleton = new WorkflowsQActionTable();

		protected WorkflowsQActionTable()
			: base(Parameter.Repositoryworkflows.tablePid, Parameter.Repositoryworkflows.indexColumn, Parameter.Repositoryworkflows.indexColumnPid)
		{
			PollManagerQActionTable.Singleton.StateChanged += PollManager_StateChanged;
			RepositoriesQActionTable.Singleton.RowDeleted += Repositories_RowDeleted;

			ID = new SLReadColumn<string>(
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsid_1601,
				Parameter.Repositoryworkflows.Pid.repositoryworkflowsid_1601,
				this);

			RepositoryID = new SLReadColumn<string>(
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsrepositoryid_1602,
				Parameter.Repositoryworkflows.Pid.repositoryworkflowsrepositoryid_1602,
				this);

			Name = new SLReadColumn<string>(
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsname_1603,
				Parameter.Repositoryworkflows.Pid.repositoryworkflowsname_1603,
				this);

			State = new SLReadColumn<string>(
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsstate_1604,
				Parameter.Repositoryworkflows.Pid.repositoryworkflowsstate_1604,
				this);

			Path = new SLReadColumn<string>(
				Parameter.Repositoryworkflows.Idx.repositoryworkflowspath_1605,
				Parameter.Repositoryworkflows.Pid.repositoryworkflowspath_1605,
				this);

			CreatedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryworkflows.Idx.repositoryworkflowscreatedat_1606,
				Parameter.Repositoryworkflows.Pid.repositoryworkflowscreatedat_1606,
				this);

			UpdatedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsupdatedat_1607,
				Parameter.Repositoryworkflows.Pid.repositoryworkflowsupdatedat_1607,
				this);

			DeletedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsdeletedat_1608,
				Parameter.Repositoryworkflows.Pid.repositoryworkflowsdeletedat_1608,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryworkflows.Idx.repositoryworkflowslastpolledatutc_1599,
				Parameter.Repositoryworkflows.Pid.repositoryworkflowslastpolledatutc_1599,
				this);
		}

		public SLReadColumn<string> ID { get; }

		public SLReadColumn<string> RepositoryID { get; }

		public SLReadColumn<string> Name { get; }

		public SLReadColumn<string> State { get; }

		public SLReadColumn<string> Path { get; }

		public SLReadColumn<DateTime?> CreatedAt { get; }

		public SLReadColumn<DateTime?> UpdatedAt { get; }

		public SLReadColumn<DateTime?> DeletedAt { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol, string repositoryId)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_Workflows)));
			var toBeRemoved = SLTables.Workflows.GetData(
				protocol,
				ID.Read.Map<WorkflowsModel>(m => m.ID),
				RepositoryID.Read.Map<WorkflowsModel>(m => m.RepositoryID),
				LastPolledAt.Read.Map<WorkflowsModel>(m => m.LastPolledAt))
					.Where(m => m.RepositoryID == repositoryId)
					.Where(m => m.LastPolledAt < pollRow.LastPolledUTCTime)
					.Select(m => m.ID)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		public void Cleanup(SLProtocol protocol)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_Workflows)));
			if (!pollRow.PreviouslyPolledUTCTime.HasValue)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(MembersQActionTable)}.{nameof(Cleanup)}|Table hasn't been polled twice yet", LogType.DebugInfo, LogLevel.Level2);
				return;
			}

			var toBeRemoved = SLTables.Workflows.GetData(
				protocol,
				ID.Read.Map<WorkflowsModel>(m => m.ID),
				LastPolledAt.Read.Map<WorkflowsModel>(m => m.LastPolledAt))
					.Where(m =>
						!m.LastPolledAt.HasValue ||
						(m.LastPolledAt < pollRow.PreviouslyPolledUTCTime))
					.Select(m => m.ID)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		protected override void DisposeEvents()
		{
			PollManagerQActionTable.Singleton.StateChanged -= PollManager_StateChanged;
			RepositoriesQActionTable.Singleton.RowDeleted -= Repositories_RowDeleted;
		}

		private void PollManager_StateChanged(object sender, Poll_Manager.PollManagerStateChangedEventArgs e)
		{
			if (e.RequestType != RequestType.Repositories_Workflows)
			{
				return;
			}

			if (e.PollState != PollState.Disabled)
			{
				return;
			}

			ClearAllKeys(e.Protocol);
		}

		private void Repositories_RowDeleted(object sender, DeleteRowEventArgs e)
		{
			var toBeRemoved = SLTables.Workflows.GetData(
				e.Protocol,
				ID.Read.Map<WorkflowsModel>(m => m.ID),
				RepositoryID.Read.Map<WorkflowsModel>(m => m.RepositoryID))
					.Where(m => e.PrimaryKeys.Contains(m.RepositoryID))
					.Select(m => m.ID)
					.ToHashSet();

			DeleteRows(e.Protocol, toBeRemoved);
		}
	}
}
