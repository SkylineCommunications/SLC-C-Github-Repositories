// Ignore Spelling: Spdx

namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.DataMiner.Scripting.Helper.Events;
	using Skyline.Protocol.PollManager;

	public class SoftwareBillOfMaterialsRowConverter : ISLRowConverter<SoftwareBillOfMaterialsModel, RepositorysoftwarebillofmaterialsQActionRow>
	{
		public static readonly SoftwareBillOfMaterialsRowConverter Instance = new SoftwareBillOfMaterialsRowConverter();

		private readonly ColumnMapBase<SoftwareBillOfMaterialsModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<SoftwareBillOfMaterialsModel>[] _columnWriteMaps;

		protected SoftwareBillOfMaterialsRowConverter()
		{
			_columnMaps = new ColumnMapBase<SoftwareBillOfMaterialsModel>[]
			{
				SLTables.SoftwareBillOfMaterials.Name.Read.Map<SoftwareBillOfMaterialsModel>(m => m.Name),
				SLTables.SoftwareBillOfMaterials.RepositoryID.Read.Map<SoftwareBillOfMaterialsModel>(m => m.RepositoryID),
				SLTables.SoftwareBillOfMaterials.SpdxId.Read.Map<SoftwareBillOfMaterialsModel>(m => m.SpdxId),
				SLTables.SoftwareBillOfMaterials.SpdxVersion.Read.Map<SoftwareBillOfMaterialsModel>(m => m.SpdxVersion),
				SLTables.SoftwareBillOfMaterials.DataLicense.Read.Map<SoftwareBillOfMaterialsModel>(m => m.DataLicense),
				SLTables.SoftwareBillOfMaterials.CreatedAt.Read.Map<SoftwareBillOfMaterialsModel>(m => m.CreatedAt),
				SLTables.SoftwareBillOfMaterials.DocumentNamespace.Read.Map<SoftwareBillOfMaterialsModel>(m => m.DocumentNamespace),
				SLTables.SoftwareBillOfMaterials.LastPolledAt.Read.Map<SoftwareBillOfMaterialsModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<SoftwareBillOfMaterialsModel>[]
			{
				SLTables.SoftwareBillOfMaterials.Name.Read.MapWrite<SoftwareBillOfMaterialsModel>(m => m.Name),
				SLTables.SoftwareBillOfMaterials.RepositoryID.Read.MapWrite<SoftwareBillOfMaterialsModel>(m => m.RepositoryID),
				SLTables.SoftwareBillOfMaterials.SpdxId.Read.MapWrite<SoftwareBillOfMaterialsModel>(m => m.SpdxId),
				SLTables.SoftwareBillOfMaterials.SpdxVersion.Read.MapWrite<SoftwareBillOfMaterialsModel>(m => m.SpdxVersion),
				SLTables.SoftwareBillOfMaterials.DataLicense.Read.MapWrite<SoftwareBillOfMaterialsModel>(m => m.DataLicense),
				SLTables.SoftwareBillOfMaterials.CreatedAt.Read.MapWrite<SoftwareBillOfMaterialsModel>(m => m.CreatedAt),
				SLTables.SoftwareBillOfMaterials.DocumentNamespace.Read.MapWrite<SoftwareBillOfMaterialsModel>(m => m.DocumentNamespace),
				SLTables.SoftwareBillOfMaterials.LastPolledAt.Read.MapWrite<SoftwareBillOfMaterialsModel>(m => m.LastPolledAt),
			};
		}

		public SoftwareBillOfMaterialsModel FromRawValue(RepositorysoftwarebillofmaterialsQActionRow rawRow)
		{
			var row = new SoftwareBillOfMaterialsModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public RepositorysoftwarebillofmaterialsQActionRow ToRawValue(SoftwareBillOfMaterialsModel row)
		{
			var qactionRow = new RepositorysoftwarebillofmaterialsQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new RepositorysoftwarebillofmaterialsQActionRow(rawRow);
		}
	}

	public class SoftwareBillOfMaterialsModel
	{
		public string Name { get; set; }

		public string RepositoryID { get; set; }

		public string SpdxId { get; set; }

		public string SpdxVersion { get; set; }

		public string DataLicense { get; set; }

		public DateTime? CreatedAt { get; set; }

		public string DocumentNamespace { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class SoftwareBillOfMaterialsQActionTable : SLTable<RepositorysoftwarebillofmaterialsQActionRow>
	{
		public static readonly SoftwareBillOfMaterialsQActionTable Singleton = new SoftwareBillOfMaterialsQActionTable();

		protected SoftwareBillOfMaterialsQActionTable()
			: base(Parameter.Repositorysoftwarebillofmaterials.tablePid, Parameter.Repositorysoftwarebillofmaterials.indexColumn, Parameter.Repositorysoftwarebillofmaterials.indexColumnPid)
		{
			PollManagerQActionTable.Singleton.StateChanged += PollManager_StateChanged;
			RepositoriesQActionTable.Singleton.RowDeleted += Repositories_RowDeleted;

			Name = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsname_2201,
				Parameter.Repositorysoftwarebillofmaterials.Pid.repositorysoftwarebillofmaterialsname_2201,
				this);

			RepositoryID = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsrepositoryid_2202,
				Parameter.Repositorysoftwarebillofmaterials.Pid.repositorysoftwarebillofmaterialsrepositoryid_2202,
				this);

			SpdxId = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsspdxid_2203,
				Parameter.Repositorysoftwarebillofmaterials.Pid.repositorysoftwarebillofmaterialsspdxid_2203,
				this);

			SpdxVersion = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsspdxversion_2204,
				Parameter.Repositorysoftwarebillofmaterials.Pid.repositorysoftwarebillofmaterialsspdxversion_2204,
				this);

			DataLicense = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsdatalicense_2205,
				Parameter.Repositorysoftwarebillofmaterials.Pid.repositorysoftwarebillofmaterialsdatalicense_2205,
				this);

			CreatedAt = new SLReadColumn<DateTime?>(
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialscreatedatutc_2206,
				Parameter.Repositorysoftwarebillofmaterials.Pid.repositorysoftwarebillofmaterialscreatedatutc_2206,
				this);

			DocumentNamespace = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsdocumentnamespace_2207,
				Parameter.Repositorysoftwarebillofmaterials.Pid.repositorysoftwarebillofmaterialsdocumentnamespace_2207,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialslastpolledatutc_2199,
				Parameter.Repositorysoftwarebillofmaterials.Pid.repositorysoftwarebillofmaterialslastpolledatutc_2199,
				this);
		}

		public SLReadColumn<string> Name { get; }

		public SLReadColumn<string> RepositoryID { get; }

		public SLReadColumn<string> SpdxId { get; }

		public SLReadColumn<string> SpdxVersion { get; }

		public SLReadColumn<string> DataLicense { get; }

		public SLReadColumn<DateTime?> CreatedAt { get; }

		public SLReadColumn<string> DocumentNamespace { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol, string repositoryId)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_SoftwareBillOfMaterials)));
			var toBeRemoved = SLTables.SoftwareBillOfMaterials.GetData(
				protocol,
				Name.Read.Map<SoftwareBillOfMaterialsModel>(m => m.Name),
				RepositoryID.Read.Map<SoftwareBillOfMaterialsModel>(m => m.RepositoryID),
				LastPolledAt.Read.Map<SoftwareBillOfMaterialsModel>(m => m.LastPolledAt))
					.Where(m => m.RepositoryID == repositoryId)
					.Where(m => m.LastPolledAt < pollRow.LastPolledUTCTime)
					.Select(m => m.Name)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		public void Cleanup(SLProtocol protocol)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_SoftwareBillOfMaterials)));
			var pollTime = pollRow.PollingStatus == PollingStatus.Polling ? pollRow.PreviouslyPolledUTCTime : pollRow.LastPolledUTCTime;
			if (!pollTime.HasValue)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(MembersQActionTable)}.{nameof(Cleanup)}|Table hasn't been polled yet", LogType.DebugInfo, LogLevel.Level2);
				return;
			}

			var toBeRemoved = SLTables.SoftwareBillOfMaterials.GetData(
				protocol,
				Name.Read.Map<SoftwareBillOfMaterialsModel>(m => m.Name),
				LastPolledAt.Read.Map<SoftwareBillOfMaterialsModel>(m => m.LastPolledAt))
					.Where(m =>
						!m.LastPolledAt.HasValue ||
						(m.LastPolledAt < pollTime))
					.Select(m => m.Name)
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
			if (e.RequestType != RequestType.Repositories_SoftwareBillOfMaterials)
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
			var toBeRemoved = SLTables.SoftwareBillOfMaterials.GetData(
				e.Protocol,
				Name.Read.Map<SoftwareBillOfMaterialsModel>(m => m.Name),
				RepositoryID.Read.Map<SoftwareBillOfMaterialsModel>(m => m.RepositoryID))
					.Where(m => e.PrimaryKeys.Contains(m.RepositoryID))
					.Select(m => m.Name)
					.ToHashSet();

			DeleteRows(e.Protocol, toBeRemoved);
		}
	}
}
