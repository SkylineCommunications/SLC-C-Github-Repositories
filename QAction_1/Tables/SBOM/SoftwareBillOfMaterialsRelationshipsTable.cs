// Ignore Spelling: Spdx

namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.DataMiner.Scripting.Helper.Events;
	using Skyline.Protocol.PollManager;

	public class SoftwareBillOfMaterialsRelationshipsRowConverter : ISLRowConverter<SoftwareBillOfMaterialsRelationshipsModel, RepositorysoftwarebillofmaterialsrelationshipsQActionRow>
	{
		public static readonly SoftwareBillOfMaterialsRelationshipsRowConverter Instance = new SoftwareBillOfMaterialsRelationshipsRowConverter();

		private readonly ColumnMapBase<SoftwareBillOfMaterialsRelationshipsModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<SoftwareBillOfMaterialsRelationshipsModel>[] _columnWriteMaps;

		protected SoftwareBillOfMaterialsRelationshipsRowConverter()
		{
			_columnMaps = new ColumnMapBase<SoftwareBillOfMaterialsRelationshipsModel>[]
			{
				SLTables.SoftwareBillOfMaterialsRelationships.Instance.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.Instance),
				SLTables.SoftwareBillOfMaterialsRelationships.SoftwareBillOfMaterialsName.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.SoftwareBillOfMaterialsName),
				SLTables.SoftwareBillOfMaterialsRelationships.RepositoryID.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.RepositoryID),
				SLTables.SoftwareBillOfMaterialsRelationships.SpdxElementID.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.SpdxElementID),
				SLTables.SoftwareBillOfMaterialsRelationships.RelatedSpdxElementID.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.RelatedSpdxElementID),
				SLTables.SoftwareBillOfMaterialsRelationships.RelationshipType.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.RelationshipType),
				SLTables.SoftwareBillOfMaterialsRelationships.LastPolledAt.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<SoftwareBillOfMaterialsRelationshipsModel>[]
			{
				SLTables.SoftwareBillOfMaterialsRelationships.Instance.Read.MapWrite<SoftwareBillOfMaterialsRelationshipsModel>(m => m.Instance),
				SLTables.SoftwareBillOfMaterialsRelationships.SoftwareBillOfMaterialsName.Read.MapWrite<SoftwareBillOfMaterialsRelationshipsModel>(m => m.SoftwareBillOfMaterialsName),
				SLTables.SoftwareBillOfMaterialsRelationships.RepositoryID.Read.MapWrite<SoftwareBillOfMaterialsRelationshipsModel>(m => m.RepositoryID),
				SLTables.SoftwareBillOfMaterialsRelationships.SpdxElementID.Read.MapWrite<SoftwareBillOfMaterialsRelationshipsModel>(m => m.SpdxElementID),
				SLTables.SoftwareBillOfMaterialsRelationships.RelatedSpdxElementID.Read.MapWrite<SoftwareBillOfMaterialsRelationshipsModel>(m => m.RelatedSpdxElementID),
				SLTables.SoftwareBillOfMaterialsRelationships.RelationshipType.Read.MapWrite<SoftwareBillOfMaterialsRelationshipsModel>(m => m.RelationshipType),
				SLTables.SoftwareBillOfMaterialsRelationships.LastPolledAt.Read.MapWrite<SoftwareBillOfMaterialsRelationshipsModel>(m => m.LastPolledAt),
			};
		}

		public SoftwareBillOfMaterialsRelationshipsModel FromRawValue(RepositorysoftwarebillofmaterialsrelationshipsQActionRow rawRow)
		{
			var row = new SoftwareBillOfMaterialsRelationshipsModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public RepositorysoftwarebillofmaterialsrelationshipsQActionRow ToRawValue(SoftwareBillOfMaterialsRelationshipsModel row)
		{
			var qactionRow = new RepositorysoftwarebillofmaterialsrelationshipsQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new RepositorysoftwarebillofmaterialsrelationshipsQActionRow(rawRow);
		}
	}

	public class SoftwareBillOfMaterialsRelationshipsModel
	{
		public string Instance { get; set; }

		public string SoftwareBillOfMaterialsName { get; set; }

		public string RepositoryID { get; set; }

		public string SpdxElementID { get; set; }

		public string RelatedSpdxElementID { get; set; }

		public string RelationshipType { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class SoftwareBillOfMaterialsRelationshipsQActionTable : SLTable<RepositorysoftwarebillofmaterialsrelationshipsQActionRow>
	{
		public static readonly SoftwareBillOfMaterialsRelationshipsQActionTable Singleton = new SoftwareBillOfMaterialsRelationshipsQActionTable();

		protected SoftwareBillOfMaterialsRelationshipsQActionTable()
			: base(Parameter.Repositorysoftwarebillofmaterialsrelationships.tablePid, Parameter.Repositorysoftwarebillofmaterialsrelationships.indexColumn, Parameter.Repositorysoftwarebillofmaterialsrelationships.indexColumnPid)
		{
			SoftwareBillOfMaterialsQActionTable.Singleton.RowDeleted += SoftwareBillOfMaterials_RowDeleted;

			Instance = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsinstance_2601,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Pid.repositorysoftwarebillofmaterialsrelationshipsinstance_2601,
				this);

			SoftwareBillOfMaterialsName = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipssbomid_2602,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Pid.repositorysoftwarebillofmaterialsrelationshipssbomid_2602,
				this);

			RepositoryID = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsrepositoryid_2603,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Pid.repositorysoftwarebillofmaterialsrelationshipsrepositoryid_2603,
				this);

			SpdxElementID = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsspdxelementid_2604,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Pid.repositorysoftwarebillofmaterialsrelationshipsspdxelementid_2604,
				this);

			RelatedSpdxElementID = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsrelatedspdxelementid_2605,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Pid.repositorysoftwarebillofmaterialsrelationshipsrelatedspdxelementid_2605,
				this);

			RelationshipType = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsrelationshiptype_2606,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Pid.repositorysoftwarebillofmaterialsrelationshipsrelationshiptype_2606,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipslastpolledatutc_2599,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Pid.repositorysoftwarebillofmaterialsrelationshipslastpolledatutc_2599,
				this);
		}

		public SLReadColumn<string> Instance { get; }

		public SLReadColumn<string> SoftwareBillOfMaterialsName { get; }

		public SLReadColumn<string> RepositoryID { get; }

		public SLReadColumn<string> SpdxElementID { get; }

		public SLReadColumn<string> RelatedSpdxElementID { get; }

		public SLReadColumn<string> RelationshipType { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol, string repositoryId)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_SoftwareBillOfMaterials)));
			var toBeRemoved = SLTables.SoftwareBillOfMaterialsRelationships.GetData(
				protocol,
				Instance.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.Instance),
				RepositoryID.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.RepositoryID),
				LastPolledAt.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.LastPolledAt))
					.Where(m => m.RepositoryID == repositoryId)
					.Where(m => m.LastPolledAt < pollRow.LastPolledUTCTime)
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		public void Cleanup(SLProtocol protocol)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_SoftwareBillOfMaterials)));
			if (!pollRow.LastPolledUTCTime.HasValue)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(MembersQActionTable)}.{nameof(Cleanup)}|Table hasn't been polled yet", LogType.DebugInfo, LogLevel.Level2);
				return;
			}

			var toBeRemoved = SLTables.SoftwareBillOfMaterialsRelationships.GetData(
				protocol,
				Instance.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.Instance),
				LastPolledAt.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.LastPolledAt))
					.Where(m =>
						!m.LastPolledAt.HasValue ||
						(m.LastPolledAt < pollRow.LastPolledUTCTime))
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		protected override void DisposeEvents()
		{
			SoftwareBillOfMaterialsQActionTable.Singleton.RowDeleted -= SoftwareBillOfMaterials_RowDeleted;
		}

		private void SoftwareBillOfMaterials_RowDeleted(object sender, DeleteRowEventArgs e)
		{
			var toBeRemoved = SLTables.SoftwareBillOfMaterialsRelationships.GetData(
				e.Protocol,
				Instance.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.Instance),
				SoftwareBillOfMaterialsName.Read.Map<SoftwareBillOfMaterialsRelationshipsModel>(m => m.SoftwareBillOfMaterialsName))
					.Where(m => e.PrimaryKeys.Contains(m.SoftwareBillOfMaterialsName))
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(e.Protocol, toBeRemoved);
		}
	}
}
