namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.Protocol.PollManager;
	using Skyline.Protocol.Tables.Poll_Manager;

	public class TagsRowConverter : ISLRowConverter<TagsModel, RepositorytagsQActionRow>
	{
		public static readonly TagsRowConverter Instance = new TagsRowConverter();

		private readonly ColumnMapBase<TagsModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<TagsModel>[] _columnWriteMaps;

		protected TagsRowConverter()
		{
			_columnMaps = new ColumnMapBase<TagsModel>[]
			{
				SLTables.Tags.ID.Read.Map<TagsModel>(m => m.ID),
				SLTables.Tags.Name.Read.Map<TagsModel>(m => m.Name),
				SLTables.Tags.RepositoryID.Read.Map<TagsModel>(m => m.RepositoryID),
				SLTables.Tags.CommitSHA.Read.Map<TagsModel>(m => m.CommitSHA),
				SLTables.Tags.LastPolledAt.Read.Map<TagsModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<TagsModel>[]
			{
				SLTables.Tags.ID.Read.MapWrite<TagsModel>(m => m.ID),
				SLTables.Tags.Name.Read.MapWrite<TagsModel>(m => m.Name),
				SLTables.Tags.RepositoryID.Read.MapWrite<TagsModel>(m => m.RepositoryID),
				SLTables.Tags.CommitSHA.Read.MapWrite<TagsModel>(m => m.CommitSHA),
				SLTables.Tags.LastPolledAt.Read.MapWrite<TagsModel>(m => m.LastPolledAt),
			};
		}

		public TagsModel FromRawValue(RepositorytagsQActionRow rawRow)
		{
			var row = new TagsModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public RepositorytagsQActionRow ToRawValue(TagsModel row)
		{
			var qactionRow = new RepositorytagsQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new RepositorytagsQActionRow(rawRow);
		}
	}

	public class TagsModel
	{
		public string ID { get; set; }

		public string Name { get; set; }

		public string RepositoryID { get; set; }

		public string CommitSHA { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class TagsQActionTable : SLTable<RepositorytagsQActionRow>
	{
		public static readonly TagsQActionTable Singleton = new TagsQActionTable();

		protected TagsQActionTable()
			: base(Parameter.Repositorytags.tablePid, Parameter.Repositorytags.indexColumn, Parameter.Repositorytags.indexColumnPid)
		{
			PollManagerQActionTable.Singleton.StateChanged += PollManager_StateChanged;
			RepositoriesQActionTable.Singleton.RowDeleted += Repositories_RowDeleted;

			ID = new SLReadColumn<string>(
				Parameter.Repositorytags.Idx.repositorytagsid_1201,
				Parameter.Repositorytags.Pid.repositorytagsid_1201,
				this);

			Name = new SLReadColumn<string>(
				Parameter.Repositorytags.Idx.repositorytagsname_1202,
				Parameter.Repositorytags.Pid.repositorytagsname_1202,
				this);

			RepositoryID = new SLReadColumn<string>(
				Parameter.Repositorytags.Idx.repositorytagsrepositoryid_1203,
				Parameter.Repositorytags.Pid.repositorytagsrepositoryid_1203,
				this);

			CommitSHA = new SLReadColumn<string>(
				Parameter.Repositorytags.Idx.repositorytagscommitsha_1204,
				Parameter.Repositorytags.Pid.repositorytagscommitsha_1204,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Repositorytags.Idx.repositorytagslastpolledatutc_1199,
				Parameter.Repositorytags.Pid.repositorytagslastpolledatutc_1199,
				this);
		}

		public SLReadColumn<string> ID { get; }

		public SLReadColumn<string> Name { get; }

		public SLReadColumn<string> RepositoryID { get; }

		public SLReadColumn<string> CommitSHA { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol, string repositoryId)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_Tags)));
			var toBeRemoved = SLTables.Tags.GetData(
				protocol,
				ID.Read.Map<TagsModel>(m => m.ID),
				RepositoryID.Read.Map<TagsModel>(m => m.RepositoryID),
				LastPolledAt.Read.Map<TagsModel>(m => m.LastPolledAt))
					.Where(m => m.RepositoryID == repositoryId)
					.Where(m => m.LastPolledAt < pollRow.LastPolledUTCTime)
					.Select(m => m.ID)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		public void Cleanup(SLProtocol protocol)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_Tags)));

			var pollTime = pollRow.PollingStatus == PollingStatus.Polling ? pollRow.PreviouslyPolledUTCTime : pollRow.LastPolledUTCTime;
			if (!pollTime.HasValue)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(MembersQActionTable)}.{nameof(Cleanup)}|Table hasn't been polled yet", LogType.DebugInfo, LogLevel.Level2);
				return;
			}

			var toBeRemoved = SLTables.Tags.GetData(
				protocol,
				ID.Read.Map<TagsModel>(m => m.ID),
				LastPolledAt.Read.Map<TagsModel>(m => m.LastPolledAt))
					.Where(m =>
						!m.LastPolledAt.HasValue ||
						(m.LastPolledAt < pollTime))
					.Select(m => m.ID)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		protected override void DisposeEvents()
		{
			PollManagerQActionTable.Singleton.StateChanged -= PollManager_StateChanged;
			RepositoriesQActionTable.Singleton.RowDeleted -= Repositories_RowDeleted;
		}

		private void PollManager_StateChanged(object sender, PollManagerStateChangedEventArgs e)
		{
			if (e.RequestType != RequestType.Repositories_Tags)
			{
				return;
			}

			if (e.PollState != PollState.Disabled)
			{
				return;
			}

			ClearAllKeys(e.Protocol);
		}

		private void Repositories_RowDeleted(object sender, DataMiner.Scripting.Helper.Events.DeleteRowEventArgs e)
		{
			var toBeRemoved = SLTables.Tags.GetData(
				e.Protocol,
				ID.Read.Map<TagsModel>(m => m.ID),
				RepositoryID.Read.Map<TagsModel>(m => m.RepositoryID))
					.Where(m => e.PrimaryKeys.Contains(m.RepositoryID))
					.Select(m => m.ID)
					.ToHashSet();

			DeleteRows(e.Protocol, toBeRemoved);
		}
	}
}
