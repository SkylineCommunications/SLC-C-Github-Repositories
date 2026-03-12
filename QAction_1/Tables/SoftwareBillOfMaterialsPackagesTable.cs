// Ignore Spelling: Spdx

namespace Skyline.Protocol.Tables
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Helper;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.Tables.Events;

	using SLNetMessages = Skyline.DataMiner.Net.Messages;

	public class SoftwareBillOfMaterialsPackagesTableRow
	{
		private DateTime lastPolledAt;

		public SoftwareBillOfMaterialsPackagesTableRow() { }

		public SoftwareBillOfMaterialsPackagesTableRow(params object[] row)
		{
			Instance = Convert.ToString(row[0]);
			SoftwareBillOfMaterialsName = Convert.ToString(row[1]);
			RepositoryID = Convert.ToString(row[2]);
			SpdxId = Convert.ToString(row[3]);
			Name = Convert.ToString(row[4]);
			Version = Convert.ToString(row[5]);
			DownloadLocation = Convert.ToString(row[6]);
			FilesAnalyzed = Convert.ToBoolean(row[7]);
			LicenseConcluded = Convert.ToString(row[8]);
			LicenseDeclared = Convert.ToString(row[9]);
			Supplier = Convert.ToString(row[10]);
			CopyrightText = Convert.ToString(row[11]);
			LastPolledAt = DateTime.SpecifyKind(DateTime.FromOADate(Convert.ToDouble(row[12])), DateTimeKind.Utc);
		}

		public string Instance { get; set; } = Exceptions.NotAvailable;

		public string SoftwareBillOfMaterialsName { get; set; }

		public string RepositoryID { get; set; }

		public string SpdxId { get; set; }

		public string Name { get; set; }

		public string Version { get; set; }

		public string DownloadLocation { get; set; }

		public bool FilesAnalyzed { get; set; }

		public string LicenseConcluded { get; set; }

		public string LicenseDeclared { get; set; }

		public string Supplier { get; set; }

		public string CopyrightText { get; set; }

		public DateTime LastPolledAt
		{
			get
			{
				return lastPolledAt;
			}

			set
			{
				if (value.Kind != DateTimeKind.Utc)
				{
					throw new ArgumentException("LastPolledAt must be in UTC.");
				}

				lastPolledAt = value;
			}
		}

		public static SoftwareBillOfMaterialsPackagesTableRow FromPK(SLProtocol protocol, string pk)
		{
			var row = (object[])protocol.GetRow(Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, pk);
			if (row[0] == null)
			{
				return default;
			}

			return new SoftwareBillOfMaterialsPackagesTableRow(row);
		}

		public object[] ToProtocolRow()
		{
			return new RepositorysoftwarebillofmaterialspackagesQActionRow
			{
				Repositorysoftwarebillofmaterialspackagesinstance_2401 = Instance,
				Repositorysoftwarebillofmaterialspackagessbomid_2402 = SoftwareBillOfMaterialsName,
				Repositorysoftwarebillofmaterialspackagesrepositoryid_2403 = RepositoryID,
				Repositorysoftwarebillofmaterialspackagesspdxid_2404 = SpdxId,
				Repositorysoftwarebillofmaterialspackagesname_2405 = Name,
				Repositorysoftwarebillofmaterialspackagesversion_2406 = Version,
				Repositorysoftwarebillofmaterialspackagesdownloadlocation_2407 = DownloadLocation,
				Repositorysoftwarebillofmaterialspackagesfilesanalyzed_2408 = Convert.ToInt16(FilesAnalyzed),
				Repositorysoftwarebillofmaterialspackageslicenseconcluded_2409 = LicenseConcluded,
				Repositorysoftwarebillofmaterialspackageslicensedeclared_2410 = LicenseDeclared,
				Repositorysoftwarebillofmaterialspackagessupplier_2411 = Supplier,
				Repositorysoftwarebillofmaterialspackagescopyrighttext_2412 = CopyrightText,
				Repositorysoftwarebillofmaterialspackageslastpolledatutc_2399 = LastPolledAt.ToOADate(),
			};
		}

		public void SaveToProtocol(SLProtocol protocol)
		{
			if (!protocol.Exists(Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, Instance))
			{
				protocol.AddRow(Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, ToProtocolRow());
			}
			else
			{
				protocol.SetRow(Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, Instance, ToProtocolRow());
			}
		}
	}

	public class SoftwareBillOfMaterialsPackagesTable : IDisposable
	{
		private static SoftwareBillOfMaterialsPackagesTable instance = new SoftwareBillOfMaterialsPackagesTable();

		public SoftwareBillOfMaterialsPackagesTable()
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;
			SoftwareBillOfMaterialsTable.SoftwareBillOfMaterialsChanged += SoftwareBillOfMaterialsTable_SoftwareBillOfMaterialsChanged;
		}

		public SoftwareBillOfMaterialsPackagesTable(SLProtocol protocol)
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;
			SoftwareBillOfMaterialsTable.SoftwareBillOfMaterialsChanged += SoftwareBillOfMaterialsTable_SoftwareBillOfMaterialsChanged;

			uint[] organizationTeamsIdx = new uint[]
			{
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesinstance_2401,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagessbomid_2402,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesrepositoryid_2403,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesspdxid_2404,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesname_2405,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesversion_2406,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesdownloadlocation_2407,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesfilesanalyzed_2408,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackageslicenseconcluded_2409,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackageslicensedeclared_2410,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagessupplier_2411,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagescopyrighttext_2412,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackageslastpolledatutc_2399,
			};

			object[] teams = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, organizationTeamsIdx);
			object[] pk = (object[])teams[0];
			object[] sbomId = (object[])teams[1];
			object[] repositoryId = (object[])teams[2];
			object[] spdxId = (object[])teams[3];
			object[] name = (object[])teams[4];
			object[] version = (object[])teams[5];
			object[] downloadLocation = (object[])teams[6];
			object[] filesAnalyzed = (object[])teams[7];
			object[] licenseConcluded = (object[])teams[8];
			object[] licenseDeclared = (object[])teams[9];
			object[] supplier = (object[])teams[10];
			object[] copyrightText = (object[])teams[11];
			object[] lastPolledAt = (object[])teams[12];

			for (int i = 0; i < pk.Length; i++)
			{
				Rows.Add(new SoftwareBillOfMaterialsPackagesTableRow(
				pk[i],
				sbomId[i],
				repositoryId[i],
				spdxId[i],
				name[i],
				version[i],
				downloadLocation[i],
				filesAnalyzed[i],
				licenseConcluded[i],
				licenseDeclared[i],
				supplier[i],
				copyrightText[i],
				lastPolledAt[i]));
			}
		}

		public static event EventHandler<TableEventArgs> SoftwareBillOfMaterialsPackagesChanged;

		public List<SoftwareBillOfMaterialsPackagesTableRow> Rows { get; set; } = new List<SoftwareBillOfMaterialsPackagesTableRow>();

		public static SoftwareBillOfMaterialsPackagesTable GetTable(SLProtocol protocol = null)
		{
			if (protocol is null)
			{
				return instance;
			}

			instance?.Dispose();
			instance = new SoftwareBillOfMaterialsPackagesTable(protocol);
			return instance;
		}

		public void SaveToProtocol(SLProtocol protocol, bool partial = false)
		{
			// Calculate the batch size, recommended 25000 cells max per fill array, divided by the number of columns.
			var batchSize = 25000 / 13;

			// If full then the first batch needs to be a SaveOption.Full.
			var first = !partial;
			if (!Rows.Any() && !partial)
			{
				protocol.ClearAllKeys(Parameter.Repositorysoftwarebillofmaterialspackages.tablePid);
				return;
			}

			foreach (var batch in Rows.Select(x => x.ToProtocolRow()).Batch(batchSize))
			{
				if (first)
				{
					protocol.FillArray(Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, batch.ToList(), NotifyProtocol.SaveOption.Full);
				}
				else
				{
					protocol.FillArray(Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, batch.ToList(), NotifyProtocol.SaveOption.Partial);
				}
			}
		}

		public void DeleteRow(SLProtocol protocol, params string[] rowsToDelete)
		{
			if (rowsToDelete.Length <= 0)
				return;

			// Remove from DataMiner and local instance
			protocol.DeleteRow(Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, rowsToDelete);
			instance.Rows.RemoveAll(x => rowsToDelete.ToHashSet().Contains(x.Instance));
			SoftwareBillOfMaterialsPackagesChanged?.Invoke(instance, new TableEventArgs(protocol, TableChange.Remove, rowsToDelete));
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
			SoftwareBillOfMaterialsTable.SoftwareBillOfMaterialsChanged -= SoftwareBillOfMaterialsTable_SoftwareBillOfMaterialsChanged;
		}
		#endregion

		private void RepositoriesTable_RepositoriesChanged(object sender, RepositoryEventArgs e)
		{
			// There only needs to happen something when removing a repository
			if (e.Type != RepositoryChange.Remove)
				return;

			var idx = new uint[]
			{
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesinstance_2401,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesrepositoryid_2403,
			};

			var toBeRemoved = ((object[])e.Protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, idx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => e.Repositories.Contains(row[1]))
				.Select(row => row[0])
				.ToArray();

			DeleteRow(e.Protocol, toBeRemoved);
		}

		private void SoftwareBillOfMaterialsTable_SoftwareBillOfMaterialsChanged(object sender, TableEventArgs e)
		{
			// There only needs to happen something when removing a repository
			if (e.Type != TableChange.Remove)
				return;

			var idx = new uint[]
			{
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagesinstance_2401,
				Parameter.Repositorysoftwarebillofmaterialspackages.Idx.repositorysoftwarebillofmaterialspackagessbomid_2402,
			};

			var toBeRemoved = ((object[])e.Protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, idx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => e.PrimaryKeys.Contains(row[1]))
				.Select(row => row[0])
				.ToArray();

			DeleteRow(e.Protocol, toBeRemoved);
		}
	}
}
