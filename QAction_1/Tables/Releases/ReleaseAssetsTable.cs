// Ignore Spelling: Uploader

namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.DataMiner.Scripting.Helper.Events;
	using Skyline.Protocol.PollManager;

	public class ReleaseAssetsRowConverter : ISLRowConverter<ReleaseAssetsModel, RepositoryreleaseassetsQActionRow>
	{
		public static readonly ReleaseAssetsRowConverter Instance = new ReleaseAssetsRowConverter();

		private readonly ColumnMapBase<ReleaseAssetsModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<ReleaseAssetsModel>[] _columnWriteMaps;

		protected ReleaseAssetsRowConverter()
		{
			_columnMaps = new ColumnMapBase<ReleaseAssetsModel>[]
			{
				SLTables.ReleaseAssets.Instance.Read.Map<ReleaseAssetsModel>(m => m.Instance),
				SLTables.ReleaseAssets.AssetId.Read.Map<ReleaseAssetsModel>(m => m.AssetId),
				SLTables.ReleaseAssets.RepositoryID.Read.Map<ReleaseAssetsModel>(m => m.RepositoryID),
				SLTables.ReleaseAssets.Release.Read.Map<ReleaseAssetsModel>(m => m.Release),
				SLTables.ReleaseAssets.Uploader.Read.Map<ReleaseAssetsModel>(m => m.Uploader),
				SLTables.ReleaseAssets.NodeID.Read.Map<ReleaseAssetsModel>(m => m.NodeID),
				SLTables.ReleaseAssets.Name.Read.Map<ReleaseAssetsModel>(m => m.Name),
				SLTables.ReleaseAssets.Label.Read.Map<ReleaseAssetsModel>(m => m.Label),
				SLTables.ReleaseAssets.ContentType.Read.Map<ReleaseAssetsModel>(m => m.ContentType),
				SLTables.ReleaseAssets.State.Read.Map<ReleaseAssetsModel>(m => m.State),
				SLTables.ReleaseAssets.Size.Read.Map<ReleaseAssetsModel>(m => m.Size),
				SLTables.ReleaseAssets.DownloadCount.Read.Map<ReleaseAssetsModel>(m => m.DownloadCount),
				SLTables.ReleaseAssets.CreatedAt.Read.Map<ReleaseAssetsModel>(m => m.CreatedAt),
				SLTables.ReleaseAssets.UpdatedAt.Read.Map<ReleaseAssetsModel>(m => m.UpdatedAt),
				SLTables.ReleaseAssets.BrowserDownloadUrl.Read.Map<ReleaseAssetsModel>(m => m.BrowserDownloadUrl),
				SLTables.ReleaseAssets.LastPolledAt.Read.Map<ReleaseAssetsModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<ReleaseAssetsModel>[]
			{
				SLTables.ReleaseAssets.Instance.Read.MapWrite<ReleaseAssetsModel>(m => m.Instance),
				SLTables.ReleaseAssets.AssetId.Read.MapWrite<ReleaseAssetsModel>(m => m.AssetId),
				SLTables.ReleaseAssets.RepositoryID.Read.MapWrite<ReleaseAssetsModel>(m => m.RepositoryID),
				SLTables.ReleaseAssets.Release.Read.MapWrite<ReleaseAssetsModel>(m => m.Release),
				SLTables.ReleaseAssets.Uploader.Read.MapWrite<ReleaseAssetsModel>(m => m.Uploader),
				SLTables.ReleaseAssets.NodeID.Read.MapWrite<ReleaseAssetsModel>(m => m.NodeID),
				SLTables.ReleaseAssets.Name.Read.MapWrite<ReleaseAssetsModel>(m => m.Name),
				SLTables.ReleaseAssets.Label.Read.MapWrite<ReleaseAssetsModel>(m => m.Label),
				SLTables.ReleaseAssets.ContentType.Read.MapWrite<ReleaseAssetsModel>(m => m.ContentType),
				SLTables.ReleaseAssets.State.Read.MapWrite<ReleaseAssetsModel>(m => m.State),
				SLTables.ReleaseAssets.Size.Read.MapWrite<ReleaseAssetsModel>(m => m.Size),
				SLTables.ReleaseAssets.DownloadCount.Read.MapWrite<ReleaseAssetsModel>(m => m.DownloadCount),
				SLTables.ReleaseAssets.CreatedAt.Read.MapWrite<ReleaseAssetsModel>(m => m.CreatedAt),
				SLTables.ReleaseAssets.UpdatedAt.Read.MapWrite<ReleaseAssetsModel>(m => m.UpdatedAt),
				SLTables.ReleaseAssets.BrowserDownloadUrl.Read.MapWrite<ReleaseAssetsModel>(m => m.BrowserDownloadUrl),
				SLTables.ReleaseAssets.LastPolledAt.Read.MapWrite<ReleaseAssetsModel>(m => m.LastPolledAt),
			};
		}

		public ReleaseAssetsModel FromRawValue(RepositoryreleaseassetsQActionRow rawRow)
		{
			var row = new ReleaseAssetsModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public RepositoryreleaseassetsQActionRow ToRawValue(ReleaseAssetsModel row)
		{
			var qactionRow = new RepositoryreleaseassetsQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new RepositoryreleaseassetsQActionRow(rawRow);
		}
	}

	public class ReleaseAssetsModel
	{
		public string Instance { get; set; }

		public long? AssetId { get; set; }

		public string RepositoryID { get; set; }

		public string Release { get; set; }

		public string Uploader { get; set; }

		public string NodeID { get; set; }

		public string Name { get; set; }

		public string Label { get; set; }

		public string ContentType { get; set; }

		public string State { get; set; }

		public long? Size { get; set; }

		public long? DownloadCount { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? UpdatedAt { get; set; }

		public string BrowserDownloadUrl { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class ReleaseAssetsQActionTable : SLTable<RepositoryreleaseassetsQActionRow>
	{
		public static readonly ReleaseAssetsQActionTable Singleton = new ReleaseAssetsQActionTable();

		protected ReleaseAssetsQActionTable()
			: base(Parameter.Repositoryreleaseassets.tablePid, Parameter.Repositoryreleaseassets.indexColumn, Parameter.Repositoryreleaseassets.indexColumnPid)
		{
			PollManagerQActionTable.Singleton.StateChanged += PollManager_StateChanged;
			ReleasesQActionTable.Singleton.RowDeleted += RepositoryReleases_RowDeleted;

			Instance = new SLReadColumn<string>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsinstance_1801,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetsinstance_1801,
				this);

			AssetId = new SLReadColumn<long?>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsid_1802,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetsid_1802,
				this);

			RepositoryID = new SLReadColumn<string>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsrepositoryid_1803,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetsrepositoryid_1803,
				this);

			Release = new SLReadColumn<string>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsrelease_1804,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetsrelease_1804,
				this);

			Uploader = new SLReadColumn<string>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsuploader_1805,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetsuploader_1805,
				this);

			NodeID = new SLReadColumn<string>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsnodeid_1806,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetsnodeid_1806,
				this);

			Name = new SLReadColumn<string>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsname_1807,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetsname_1807,
				this);

			Label = new SLReadColumn<string>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetslabel_1808,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetslabel_1808,
				this);

			ContentType = new SLReadColumn<string>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetscontenttype_1809,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetscontenttype_1809,
				this);

			State = new SLReadColumn<string>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsstate_1810,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetsstate_1810,
				this);

			Size = new SLReadColumn<long?>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetssize_1811,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetssize_1811,
				this);

			DownloadCount = new SLReadColumn<long?>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsdownloadcount_1812,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetsdownloadcount_1812,
				this);

			CreatedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetscreatedat_1813,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetscreatedat_1813,
				this);

			UpdatedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsupdatedat_1814,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetsupdatedat_1814,
				this);

			BrowserDownloadUrl = new SLReadColumn<string>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetsbrowserdownloadurl_1815,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetsbrowserdownloadurl_1815,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Repositoryreleaseassets.Idx.repositoryreleaseassetslastpolledatutc_1799,
				Parameter.Repositoryreleaseassets.Pid.repositoryreleaseassetslastpolledatutc_1799,
				this);
		}

		public SLReadColumn<string> Instance { get; }

		public SLReadColumn<long?> AssetId { get; }

		public SLReadColumn<string> RepositoryID { get; }

		public SLReadColumn<string> Release { get; }

		public SLReadColumn<string> Uploader { get; }

		public SLReadColumn<string> NodeID { get; }

		public SLReadColumn<string> Name { get; }

		public SLReadColumn<string> Label { get; }

		public SLReadColumn<string> ContentType { get; }

		public SLReadColumn<string> State { get; }

		public SLReadColumn<long?> Size { get; }

		public SLReadColumn<long?> DownloadCount { get; }

		public SLReadColumn<DateTime?> CreatedAt { get; }

		public SLReadColumn<DateTime?> UpdatedAt { get; }

		public SLReadColumn<string> BrowserDownloadUrl { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol, string repositoryId)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_Releases)));
			var toBeRemoved = SLTables.ReleaseAssets.GetData(
				protocol,
				Instance.Read.Map<ReleaseAssetsModel>(m => m.Instance),
				RepositoryID.Read.Map<ReleaseAssetsModel>(m => m.RepositoryID),
				LastPolledAt.Read.Map<ReleaseAssetsModel>(m => m.LastPolledAt))
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

			var toBeRemoved = SLTables.ReleaseAssets.GetData(
				protocol,
				Instance.Read.Map<ReleaseAssetsModel>(m => m.Instance),
				LastPolledAt.Read.Map<ReleaseAssetsModel>(m => m.LastPolledAt))
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
			ReleasesQActionTable.Singleton.RowDeleted -= RepositoryReleases_RowDeleted;
		}

		private void PollManager_StateChanged(object sender, Poll_Manager.PollManagerStateChangedEventArgs e)
		{
			if (e.RequestType != RequestType.Repositories_Releases)
			{
				return;
			}

			if (e.PollState != PollState.Disabled)
			{
				return;
			}

			ClearAllKeys(e.Protocol);
		}

		private void RepositoryReleases_RowDeleted(object sender, DeleteRowEventArgs e)
		{
			var toBeRemoved = SLTables.ReleaseAssets.GetData(
				e.Protocol,
				Instance.Read.Map<ReleaseAssetsModel>(m => m.Instance),
				Release.Read.Map<ReleaseAssetsModel>(m => m.Release))
					.Where(m => e.PrimaryKeys.Contains(m.Release))
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(e.Protocol, toBeRemoved);
		}
	}
}
