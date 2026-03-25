// Ignore Spelling: Spdx

namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.DataMiner.Scripting.Helper.Events;
	using Skyline.Protocol.PollManager;

	public class SoftwareBillOfMaterialsPackagesRowConverter : ISLRowConverter<SoftwareBillOfMaterialsPackagesModel, RepositorysoftwarebillofmaterialspackagesQActionRow>
	{
		public static readonly SoftwareBillOfMaterialsPackagesRowConverter Instance = new SoftwareBillOfMaterialsPackagesRowConverter();

		private readonly ColumnMapBase<SoftwareBillOfMaterialsPackagesModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<SoftwareBillOfMaterialsPackagesModel>[] _columnWriteMaps;

		protected SoftwareBillOfMaterialsPackagesRowConverter()
		{
			_columnMaps = new ColumnMapBase<SoftwareBillOfMaterialsPackagesModel>[]
			{
				SLTables.SoftwareBillOfMaterialsPackages.Instance.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.Instance),
				SLTables.SoftwareBillOfMaterialsPackages.SoftwareBillOfMaterialsName.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.SoftwareBillOfMaterialsName),
				SLTables.SoftwareBillOfMaterialsPackages.RepositoryID.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.RepositoryID),
				SLTables.SoftwareBillOfMaterialsPackages.SpdxId.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.SpdxId),
				SLTables.SoftwareBillOfMaterialsPackages.Name.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.Name),
				SLTables.SoftwareBillOfMaterialsPackages.Version.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.Version),
				SLTables.SoftwareBillOfMaterialsPackages.DownloadLocation.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.DownloadLocation),
				SLTables.SoftwareBillOfMaterialsPackages.FilesAnalyzed.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.FilesAnalyzed),
				SLTables.SoftwareBillOfMaterialsPackages.LicenseConcluded.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.LicenseConcluded),
				SLTables.SoftwareBillOfMaterialsPackages.LicenseDeclared.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.LicenseDeclared),
				SLTables.SoftwareBillOfMaterialsPackages.Supplier.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.Supplier),
				SLTables.SoftwareBillOfMaterialsPackages.CopyrightText.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.CopyrightText),
				SLTables.SoftwareBillOfMaterialsPackages.LastPolledAt.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<SoftwareBillOfMaterialsPackagesModel>[]
			{
				SLTables.SoftwareBillOfMaterialsPackages.Instance.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.Instance),
				SLTables.SoftwareBillOfMaterialsPackages.SoftwareBillOfMaterialsName.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.SoftwareBillOfMaterialsName),
				SLTables.SoftwareBillOfMaterialsPackages.RepositoryID.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.RepositoryID),
				SLTables.SoftwareBillOfMaterialsPackages.SpdxId.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.SpdxId),
				SLTables.SoftwareBillOfMaterialsPackages.Name.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.Name),
				SLTables.SoftwareBillOfMaterialsPackages.Version.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.Version),
				SLTables.SoftwareBillOfMaterialsPackages.DownloadLocation.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.DownloadLocation),
				SLTables.SoftwareBillOfMaterialsPackages.FilesAnalyzed.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.FilesAnalyzed),
				SLTables.SoftwareBillOfMaterialsPackages.LicenseConcluded.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.LicenseConcluded),
				SLTables.SoftwareBillOfMaterialsPackages.LicenseDeclared.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.LicenseDeclared),
				SLTables.SoftwareBillOfMaterialsPackages.Supplier.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.Supplier),
				SLTables.SoftwareBillOfMaterialsPackages.CopyrightText.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.CopyrightText),
				SLTables.SoftwareBillOfMaterialsPackages.LastPolledAt.Read.MapWrite<SoftwareBillOfMaterialsPackagesModel>(m => m.LastPolledAt),
			};
		}

		public SoftwareBillOfMaterialsPackagesModel FromRawValue(RepositorysoftwarebillofmaterialspackagesQActionRow rawRow)
		{
			var row = new SoftwareBillOfMaterialsPackagesModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public RepositorysoftwarebillofmaterialspackagesQActionRow ToRawValue(SoftwareBillOfMaterialsPackagesModel row)
		{
			var qactionRow = new RepositorysoftwarebillofmaterialspackagesQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new RepositorysoftwarebillofmaterialspackagesQActionRow(rawRow);
		}
	}

	public class SoftwareBillOfMaterialsPackagesModel
	{
		public string Instance { get; set; }

		public string SoftwareBillOfMaterialsName { get; set; }

		public string RepositoryID { get; set; }

		public string SpdxId { get; set; }

		public string Name { get; set; }

		public string Version { get; set; }

		public string DownloadLocation { get; set; }

		public bool? FilesAnalyzed { get; set; }

		public string LicenseConcluded { get; set; }

		public string LicenseDeclared { get; set; }

		public string Supplier { get; set; }

		public string CopyrightText { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class SoftwareBillOfMaterialsPackagesQActionTable : SLTable<RepositorysoftwarebillofmaterialspackagesQActionRow>
	{
		public static readonly SoftwareBillOfMaterialsPackagesQActionTable Singleton = new SoftwareBillOfMaterialsPackagesQActionTable();

		protected SoftwareBillOfMaterialsPackagesQActionTable()
			: base(Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, Parameter.Repositorysoftwarebillofmaterialspackages.indexColumn, Parameter.Repositorysoftwarebillofmaterialspackages.indexColumnPid)
		{
			SoftwareBillOfMaterialsQActionTable.Singleton.RowDeleted += SoftwareBillOfMaterials_RowDeleted;

			Instance = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesinstance_2401,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackagesinstance_2401,
				this);

			SoftwareBillOfMaterialsName = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagessbomid_2402,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackagessbomid_2402,
				this);

			RepositoryID = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesrepositoryid_2403,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackagesrepositoryid_2403,
				this);

			SpdxId = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesspdxid_2404,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackagesspdxid_2404,
				this);

			Name = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesname_2405,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackagesname_2405,
				this);

			Version = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesversion_2406,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackagesversion_2406,
				this);

			DownloadLocation = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesdownloadlocation_2407,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackagesdownloadlocation_2407,
				this);

			FilesAnalyzed = new SLReadColumn<bool?>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesfilesanalyzed_2408,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackagesfilesanalyzed_2408,
				this);

			LicenseConcluded = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackageslicenseconcluded_2409,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackageslicenseconcluded_2409,
				this);

			LicenseDeclared = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackageslicensedeclared_2410,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackageslicensedeclared_2410,
				this);

			Supplier = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagessupplier_2411,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackagessupplier_2411,
				this);

			CopyrightText = new SLReadColumn<string>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagescopyrighttext_2412,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackagescopyrighttext_2412,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackageslastpolledatutc_2399,
				Parameter.Repositorysoftwarebillofmaterialspackages.Pid.repositorysoftwarebillofmaterialspackageslastpolledatutc_2399,
				this);
		}

		public SLReadColumn<string> Instance { get; }

		public SLReadColumn<string> SoftwareBillOfMaterialsName { get; }

		public SLReadColumn<string> RepositoryID { get; }

		public SLReadColumn<string> SpdxId { get; }

		public SLReadColumn<string> Name { get; }

		public SLReadColumn<string> Version { get; }

		public SLReadColumn<string> DownloadLocation { get; }

		public SLReadColumn<bool?> FilesAnalyzed { get; }

		public SLReadColumn<string> LicenseConcluded { get; }

		public SLReadColumn<string> LicenseDeclared { get; }

		public SLReadColumn<string> Supplier { get; }

		public SLReadColumn<string> CopyrightText { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol, string repositoryId)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Repositories_SoftwareBillOfMaterials)));
			var toBeRemoved = SLTables.SoftwareBillOfMaterialsPackages.GetData(
				protocol,
				Instance.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.Instance),
				RepositoryID.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.RepositoryID),
				LastPolledAt.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.LastPolledAt))
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
			if (!pollRow.PreviouslyPolledUTCTime.HasValue)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(MembersQActionTable)}.{nameof(Cleanup)}|Table hasn't been polled twice yet", LogType.DebugInfo, LogLevel.Level2);
				return;
			}

			var toBeRemoved = SLTables.SoftwareBillOfMaterialsPackages.GetData(
				protocol,
				Instance.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.Instance),
				LastPolledAt.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.LastPolledAt))
					.Where(m =>
						!m.LastPolledAt.HasValue ||
						(m.LastPolledAt < pollRow.PreviouslyPolledUTCTime))
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
			var toBeRemoved = SLTables.SoftwareBillOfMaterialsPackages.GetData(
				e.Protocol,
				Instance.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.Instance),
				SoftwareBillOfMaterialsName.Read.Map<SoftwareBillOfMaterialsPackagesModel>(m => m.SoftwareBillOfMaterialsName))
					.Where(m => e.PrimaryKeys.Contains(m.SoftwareBillOfMaterialsName))
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(e.Protocol, toBeRemoved);
		}
	}
}
