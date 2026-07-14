namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.DataMiner.Scripting.Helper.Events;
	using Skyline.Protocol.PollManager;

	public class ReleasesRowConverter : ISLRowConverter<ReleasesModel, RepositoryreleasesQActionRow>
	{
		public static readonly ReleasesRowConverter Instance = new ReleasesRowConverter();

		private readonly ColumnMapBase<ReleasesModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<ReleasesModel>[] _columnWriteMaps;

		protected ReleasesRowConverter()
		{
			_columnMaps = new ColumnMapBase<ReleasesModel>[]
			{
				SLTables.Releases.Instance.Read.Map<ReleasesModel>(m => m.Instance),
				SLTables.Releases.ID.Read.Map<ReleasesModel>(m => m.ID),
				SLTables.Releases.TagName.Read.Map<ReleasesModel>(m => m.TagName),
				SLTables.Releases.TagId.Read.Map<ReleasesModel>(m => m.TagId),
				SLTables.Releases.TargetCommitish.Read.Map<ReleasesModel>(m => m.TargetCommitish),
				SLTables.Releases.Name.Read.Map<ReleasesModel>(m => m.Name),
				SLTables.Releases.Published.Read.Map<ReleasesModel>(m => m.Draft),
				SLTables.Releases.Type.Read.Map<ReleasesModel>(m => m.PreRelease),
				SLTables.Releases.Body.Read.Map<ReleasesModel>(m => m.Body),
				SLTables.Releases.Author.Read.Map<ReleasesModel>(m => m.Author),
				SLTables.Releases.CreatedAt.Read.Map<ReleasesModel>(m => m.CreatedAt),
				SLTables.Releases.PublishedAt.Read.Map<ReleasesModel>(m => m.PublishedAt),
				SLTables.Releases.RepositoryID.Read.Map<ReleasesModel>(m => m.RepositoryID),
				SLTables.Releases.LastPolledAt.Read.Map<ReleasesModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<ReleasesModel>[]
			{
				SLTables.Releases.Instance.Read.MapWrite<ReleasesModel>(m => m.Instance),
				SLTables.Releases.ID.Read.MapWrite<ReleasesModel>(m => m.ID),
				SLTables.Releases.TagName.Read.MapWrite<ReleasesModel>(m => m.TagName),
				SLTables.Releases.TagId.Read.MapWrite<ReleasesModel>(m => m.TagId),
				SLTables.Releases.TargetCommitish.Read.MapWrite<ReleasesModel>(m => m.TargetCommitish),
				SLTables.Releases.Name.Read.MapWrite<ReleasesModel>(m => m.Name),
				SLTables.Releases.Published.Read.MapWrite<ReleasesModel>(m => m.Draft),
				SLTables.Releases.Type.Read.MapWrite<ReleasesModel>(m => m.PreRelease),
				SLTables.Releases.Body.Read.MapWrite<ReleasesModel>(m => m.Body),
				SLTables.Releases.Author.Read.MapWrite<ReleasesModel>(m => m.Author),
				SLTables.Releases.CreatedAt.Read.MapWrite<ReleasesModel>(m => m.CreatedAt),
				SLTables.Releases.PublishedAt.Read.MapWrite<ReleasesModel>(m => m.PublishedAt),
				SLTables.Releases.RepositoryID.Read.MapWrite<ReleasesModel>(m => m.RepositoryID),
				SLTables.Releases.LastPolledAt.Read.MapWrite<ReleasesModel>(m => m.LastPolledAt),
			};
		}

		public ReleasesModel FromRawValue(RepositoryreleasesQActionRow rawRow)
		{
			var row = new ReleasesModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public RepositoryreleasesQActionRow ToRawValue(ReleasesModel row)
		{
			var qactionRow = new RepositoryreleasesQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new RepositoryreleasesQActionRow(rawRow);
		}
	}

	public class ReleasesModel
	{
		public string Instance { get; set; }

		public long? ID { get; set; }

		public string TagName { get; set; }

		public string TagId { get; set; }

		public string TargetCommitish { get; set; }

		public string Name { get; set; }

		public bool? Draft { get; set; }

		public bool? PreRelease { get; set; }

		public string Body { get; set; }

		public string Author { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? PublishedAt { get; set; }

		public string RepositoryID { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class ReleasesQActionTable : SLTable<RepositoryreleasesQActionRow>
	{
		public static readonly ReleasesQActionTable Singleton = new ReleasesQActionTable();

		protected ReleasesQActionTable()
			: base(Parameter.Repositoryreleases.tablePid, Parameter.Repositoryreleases.indexColumn, Parameter.Repositoryreleases.indexColumnPid)
		{
			PollManagerQActionTable.Singleton.StateChanged += PollManager_StateChanged;
			TagsQActionTable.Singleton.RowDeleted += Tags_RowDeleted;

			Instance = new SLReadColumn<string>(
				Parameter.Repositoryreleases.Idx.repositoryreleasesinstance,
				Parameter.Repositoryreleases.Pid.repositoryreleasesinstance,
				this);

			ID = new SLReadColumn<long?>(
				Parameter.Repositoryreleases.Idx.repositoryreleasesid,
				Parameter.Repositoryreleases.Pid.repositoryreleasesid,
				this);

			TagName = new SLReadColumn<string>(
				Parameter.Repositoryreleases.Idx.repositoryreleasestagname,
				Parameter.Repositoryreleases.Pid.repositoryreleasestagname,
				this);

			TagId = new SLReadColumn<string>(
				Parameter.Repositoryreleases.Idx.repositoryreleasestagid,
				Parameter.Repositoryreleases.Pid.repositoryreleasestagid,
				this);

			TargetCommitish = new SLReadColumn<string>(
				Parameter.Repositoryreleases.Idx.repositoryreleasestargetcommitish,
				Parameter.Repositoryreleases.Pid.repositoryreleasestargetcommitish,
				this);

			Name = new SLReadColumn<string>(
				Parameter.Repositoryreleases.Idx.repositoryreleasesname,
				Parameter.Repositoryreleases.Pid.repositoryreleasesname,
				this);

			Published = new SLReadColumn<bool?>(
				Parameter.Repositoryreleases.Idx.repositoryreleasespublished,
				Parameter.Repositoryreleases.Pid.repositoryreleasespublished,
				this);

			Type = new SLReadColumn<bool?>(
				Parameter.Repositoryreleases.Idx.repositoryreleasestype,
				Parameter.Repositoryreleases.Pid.repositoryreleasestype,
				this);

			Body = new SLReadColumn<string>(
				Parameter.Repositoryreleases.Idx.repositoryreleasesbody,
				Parameter.Repositoryreleases.Pid.repositoryreleasesbody,
				this);

			Author = new SLReadColumn<string>(
				Parameter.Repositoryreleases.Idx.repositoryreleasesauthor,
				Parameter.Repositoryreleases.Pid.repositoryreleasesauthor,
				this);

			CreatedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryreleases.Idx.repositoryreleasescreatedat,
				Parameter.Repositoryreleases.Pid.repositoryreleasescreatedat,
				this);

			PublishedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryreleases.Idx.repositoryreleasespublishedat,
				Parameter.Repositoryreleases.Pid.repositoryreleasespublishedat,
				this);

			RepositoryID = new SLReadColumn<string>(
				Parameter.Repositoryreleases.Idx.repositoryreleasesrepositoryid,
				Parameter.Repositoryreleases.Pid.repositoryreleasesrepositoryid,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryreleases.Idx.repositoryreleaseslastpolledatutc,
				Parameter.Repositoryreleases.Pid.repositoryreleaseslastpolledatutc,
				this);
		}

		public SLReadColumn<string> Instance { get; }

		public SLReadColumn<long?> ID { get; }

		public SLReadColumn<string> TagName { get; }

		public SLReadColumn<string> TagId { get; }

		public SLReadColumn<string> TargetCommitish { get; }

		public SLReadColumn<string> Name { get; }

		public SLReadColumn<bool?> Published { get; }

		public SLReadColumn<bool?> Type { get; }

		public SLReadColumn<string> Body { get; }

		public SLReadColumn<string> Author { get; }

		public SLReadColumn<DateTime?> CreatedAt { get; }

		public SLReadColumn<DateTime?> PublishedAt { get; }

		public SLReadColumn<string> RepositoryID { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol, string repositoryId)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_Releases)));
			var toBeRemoved = SLTables.Releases.GetData(
				protocol,
				Instance.Read.Map<ReleasesModel>(m => m.Instance),
				RepositoryID.Read.Map<ReleasesModel>(m => m.RepositoryID),
				LastPolledAt.Read.Map<ReleasesModel>(m => m.LastPolledAt))
					.Where(m => m.RepositoryID == repositoryId)
					.Where(m => m.LastPolledAt < pollRow.LastPolledUTCTime)
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		public void Cleanup(SLProtocol protocol)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_Releases)));

			var pollTime = pollRow.PollingStatus == PollingStatus.Polling ? pollRow.PreviouslyPolledUTCTime : pollRow.LastPolledUTCTime;

			if (!pollTime.HasValue)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(MembersQActionTable)}.{nameof(Cleanup)}|Table hasn't been polled yet", LogType.DebugInfo, LogLevel.Level2);
				return;
			}

			var toBeRemoved = SLTables.Releases.GetData(
				protocol,
				Instance.Read.Map<ReleasesModel>(m => m.Instance),
				LastPolledAt.Read.Map<ReleasesModel>(m => m.LastPolledAt))
					.Where(m =>
						!m.LastPolledAt.HasValue ||
						(m.LastPolledAt < pollTime))
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		protected override void DisposeEvents()
		{
			PollManagerQActionTable.Singleton.StateChanged -= PollManager_StateChanged;
			TagsQActionTable.Singleton.RowDeleted -= Tags_RowDeleted;
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

		private void Tags_RowDeleted(object sender, DeleteRowEventArgs e)
		{
			var toBeRemoved = SLTables.Releases.GetData(
				e.Protocol,
				Instance.Read.Map<ReleasesModel>(m => m.Instance),
				TagId.Read.Map<ReleasesModel>(m => m.TagId))
					.Where(m => e.PrimaryKeys.Contains(m.TagId))
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(e.Protocol, toBeRemoved);
		}
	}
}
