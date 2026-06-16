namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.DataMiner.Scripting.Helper.Events;
	using Skyline.Protocol.PollManager;

	public class IssuesRowConverter : ISLRowConverter<IssuesModel, RepositoryissuesQActionRow>
	{
		public static readonly IssuesRowConverter Instance = new IssuesRowConverter();

		private readonly ColumnMapBase<IssuesModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<IssuesModel>[] _columnWriteMaps;

		protected IssuesRowConverter()
		{
			_columnMaps = new ColumnMapBase<IssuesModel>[]
			{
				SLTables.Issues.Instance.Read.Map<IssuesModel>(m => m.Instance),
				SLTables.Issues.Number.Read.Map<IssuesModel>(m => m.Number),
				SLTables.Issues.Title.Read.Map<IssuesModel>(m => m.Title),
				SLTables.Issues.Body.Read.Map<IssuesModel>(m => m.Body),
				SLTables.Issues.Creator.Read.Map<IssuesModel>(m => m.Creator),
				SLTables.Issues.State.Read.Map<IssuesModel>(m => m.State),
				SLTables.Issues.Assignee.Read.Map<IssuesModel>(m => m.Assignee),
				SLTables.Issues.CreatedAt.Read.Map<IssuesModel>(m => m.CreatedAt),
				SLTables.Issues.UpdatedAt.Read.Map<IssuesModel>(m => m.UpdatedAt),
				SLTables.Issues.ClosedAt.Read.Map<IssuesModel>(m => m.ClosedAt),
				SLTables.Issues.RepositoryID.Read.Map<IssuesModel>(m => m.RepositoryID),
				SLTables.Issues.LastPolledAt.Read.Map<IssuesModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<IssuesModel>[]
			{
				SLTables.Issues.Instance.Read.MapWrite<IssuesModel>(m => m.Instance),
				SLTables.Issues.Number.Read.MapWrite<IssuesModel>(m => m.Number),
				SLTables.Issues.Title.Read.MapWrite<IssuesModel>(m => m.Title),
				SLTables.Issues.Body.Read.MapWrite<IssuesModel>(m => m.Body),
				SLTables.Issues.Creator.Read.MapWrite<IssuesModel>(m => m.Creator),
				SLTables.Issues.State.Read.MapWrite<IssuesModel>(m => m.State),
				SLTables.Issues.Assignee.Read.MapWrite<IssuesModel>(m => m.Assignee),
				SLTables.Issues.CreatedAt.Read.MapWrite<IssuesModel>(m => m.CreatedAt),
				SLTables.Issues.UpdatedAt.Read.MapWrite<IssuesModel>(m => m.UpdatedAt),
				SLTables.Issues.ClosedAt.Read.MapWrite<IssuesModel>(m => m.ClosedAt),
				SLTables.Issues.RepositoryID.Read.MapWrite<IssuesModel>(m => m.RepositoryID),
				SLTables.Issues.LastPolledAt.Read.MapWrite<IssuesModel>(m => m.LastPolledAt),
			};
		}

		public IssuesModel FromRawValue(RepositoryissuesQActionRow rawRow)
		{
			var row = new IssuesModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public RepositoryissuesQActionRow ToRawValue(IssuesModel row)
		{
			var qactionRow = new RepositoryissuesQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new RepositoryissuesQActionRow(rawRow);
		}
	}

	public class IssuesModel
	{
		public string Instance { get; set; }

		public int? Number { get; set; }

		public string Title { get; set; }

		public string Body { get; set; }

		public string Creator { get; set; }

		public IssueState State { get; set; }

		public string Assignee { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public DateTime? ClosedAt { get; set; }

		public string RepositoryID { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class IssuesQActionTable : SLTable<RepositoryissuesQActionRow>
	{
		public static readonly IssuesQActionTable Singleton = new IssuesQActionTable();

		protected IssuesQActionTable()
			: base(Parameter.Repositoryissues.tablePid, Parameter.Repositoryissues.indexColumn, Parameter.Repositoryissues.indexColumnPid)
		{
			PollManagerQActionTable.Singleton.StateChanged += PollManager_StateChanged;
			RepositoriesQActionTable.Singleton.RowDeleted += Repositories_RowDeleted;

			Instance = new SLReadColumn<string>(
				Parameter.Repositoryissues.Idx.repositoryissuesinstance,
				Parameter.Repositoryissues.Pid.repositoryissuesinstance,
				this);

			Number = new SLReadColumn<int?>(
				Parameter.Repositoryissues.Idx.repositoryissuesnumber,
				Parameter.Repositoryissues.Pid.repositoryissuesnumber,
				this);

			Title = new SLReadColumn<string>(
				Parameter.Repositoryissues.Idx.repositoryissuestitle,
				Parameter.Repositoryissues.Pid.repositoryissuestitle,
				this);

			Body = new SLReadColumn<string>(
				Parameter.Repositoryissues.Idx.repositoryissuesbody,
				Parameter.Repositoryissues.Pid.repositoryissuesbody,
				this);

			Creator = new SLReadColumn<string>(
				Parameter.Repositoryissues.Idx.repositoryissuescreator,
				Parameter.Repositoryissues.Pid.repositoryissuescreator,
				this);

			State = new SLReadColumn<IssueState>(
				Parameter.Repositoryissues.Idx.repositoryissuesstate,
				Parameter.Repositoryissues.Pid.repositoryissuesstate,
				new IssueStateConverter(),
				this);

			Assignee = new SLReadColumn<string>(
				Parameter.Repositoryissues.Idx.repositoryissuesassignee,
				Parameter.Repositoryissues.Pid.repositoryissuesassignee,
				this);

			CreatedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryissues.Idx.repositoryissuescreatedat,
				Parameter.Repositoryissues.Pid.repositoryissuescreatedat,
				this);

			UpdatedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryissues.Idx.repositoryissuesupdatedat,
				Parameter.Repositoryissues.Pid.repositoryissuesupdatedat,
				this);

			ClosedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryissues.Idx.repositoryissuesclosedat,
				Parameter.Repositoryissues.Pid.repositoryissuesclosedat,
				this);

			RepositoryID = new SLReadColumn<string>(
				Parameter.Repositoryissues.Idx.repositoryissuesrepositoryid,
				Parameter.Repositoryissues.Pid.repositoryissuesrepositoryid,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryissues.Idx.repositoryissueslastpolledatutc,
				Parameter.Repositoryissues.Pid.repositoryissueslastpolledatutc,
				this);
		}

		public SLReadColumn<string> Instance { get; }

		public SLReadColumn<int?> Number { get; }

		public SLReadColumn<string> Title { get; }

		public SLReadColumn<string> Body { get; }

		public SLReadColumn<string> Creator { get; }

		public SLReadColumn<IssueState> State { get; }

		public SLReadColumn<string> Assignee { get; }

		public SLReadColumn<DateTime?> CreatedAt { get; }

		public SLReadColumn<DateTime?> UpdatedAt { get; }

		public SLReadColumn<DateTime?> ClosedAt { get; }

		public SLReadColumn<string> RepositoryID { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol, string repositoryId)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repository_Issues)));
			var toBeRemoved = SLTables.Issues.GetData(
				protocol,
				Instance.Read.Map<IssuesModel>(m => m.Instance),
				RepositoryID.Read.Map<IssuesModel>(m => m.RepositoryID),
				LastPolledAt.Read.Map<IssuesModel>(m => m.LastPolledAt))
					.Where(m => m.RepositoryID == repositoryId)
					.Where(m => m.LastPolledAt < pollRow.LastPolledUTCTime)
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		public void Cleanup(SLProtocol protocol)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repository_Issues)));
			if (!pollRow.LastPolledUTCTime.HasValue)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(MembersQActionTable)}.{nameof(Cleanup)}|Table hasn't been polled yet", LogType.DebugInfo, LogLevel.Level2);
				return;
			}

			var toBeRemoved = SLTables.Issues.GetData(
				protocol,
				Instance.Read.Map<IssuesModel>(m => m.Instance),
				LastPolledAt.Read.Map<IssuesModel>(m => m.LastPolledAt))
					.Where(m =>
						!m.LastPolledAt.HasValue ||
						(m.LastPolledAt < pollRow.LastPolledUTCTime))
					.Select(m => m.Instance)
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
			if (e.RequestType != RequestType.Repository_Issues)
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
			var toBeRemoved = SLTables.Issues.GetData(
				e.Protocol,
				Instance.Read.Map<IssuesModel>(m => m.Instance),
				RepositoryID.Read.Map<IssuesModel>(m => m.RepositoryID))
					.Where(m => e.PrimaryKeys.Contains(m.RepositoryID))
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(e.Protocol, toBeRemoved);
		}
	}
}
