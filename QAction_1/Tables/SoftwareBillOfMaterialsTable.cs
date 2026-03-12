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

	public class SoftwareBillOfMaterialsTableRow
	{
		private DateTime createdAt;
		private DateTime lastPolledAt;

		public SoftwareBillOfMaterialsTableRow() { }

		public SoftwareBillOfMaterialsTableRow(params object[] row)
		{
			Name = Convert.ToString(row[0]);
			RepositoryID = Convert.ToString(row[1]);
			SpdxId = Convert.ToString(row[2]);
			SpdxVersion = Convert.ToString(row[3]);
			DataLicense = Convert.ToString(row[4]);
			CreateAt = DateTime.SpecifyKind(DateTime.FromOADate(Convert.ToDouble(row[5])), DateTimeKind.Utc);
			DocumentNamespace = Convert.ToString(row[6]);
			LastPolledAt = DateTime.SpecifyKind(DateTime.FromOADate(Convert.ToDouble(row[7])), DateTimeKind.Utc);
		}

		public string Name { get; set; } = Exceptions.NotAvailable;

		public string RepositoryID { get; set; }

		public string SpdxId { get; set; }

		public string SpdxVersion { get; set; }

		public string DataLicense { get; set; }

		public string DocumentNamespace { get; set; }

		public DateTime CreateAt
		{
			get
			{
				return createdAt;
			}

			set
			{
				if (value.Kind != DateTimeKind.Utc)
				{
					throw new ArgumentException("CreateAt must be in UTC.");
				}

				createdAt = value;
			}
		}

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

		public static SoftwareBillOfMaterialsTableRow FromPK(SLProtocol protocol, string pk)
		{
			var row = (object[])protocol.GetRow(Parameter.Repositorysoftwarebillofmaterials.tablePid, pk);
			if (row[0] == null)
			{
				return default;
			}

			return new SoftwareBillOfMaterialsTableRow(row);
		}

		public object[] ToProtocolRow()
		{
			return new RepositorysoftwarebillofmaterialsQActionRow
			{
				Repositorysoftwarebillofmaterialsname_2201 = Name,
				Repositorysoftwarebillofmaterialsrepositoryid_2202 = RepositoryID,
				Repositorysoftwarebillofmaterialsspdxid_2203 = SpdxId,
				Repositorysoftwarebillofmaterialsspdxversion_2204 = SpdxVersion,
				Repositorysoftwarebillofmaterialsdatalicense_2205 = DataLicense,
				Repositorysoftwarebillofmaterialscreatedatutc_2206 = CreateAt.ToOADate(),
				Repositorysoftwarebillofmaterialsdocumentnamespace_2207 = DocumentNamespace,
				Repositorysoftwarebillofmaterialslastpolledatutc_2199 = LastPolledAt.ToOADate(),
			};
		}

		public void SaveToProtocol(SLProtocol protocol)
		{
			if (!protocol.Exists(Parameter.Repositorysoftwarebillofmaterials.tablePid, Name))
			{
				protocol.AddRow(Parameter.Repositorysoftwarebillofmaterials.tablePid, ToProtocolRow());
			}
			else
			{
				protocol.SetRow(Parameter.Repositorysoftwarebillofmaterials.tablePid, Name, ToProtocolRow());
			}
		}
	}

	public class SoftwareBillOfMaterialsTable : IDisposable
	{
		private static SoftwareBillOfMaterialsTable instance = new SoftwareBillOfMaterialsTable();

		public SoftwareBillOfMaterialsTable()
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;
		}

		public SoftwareBillOfMaterialsTable(SLProtocol protocol)
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;

			uint[] organizationTeamsIdx = new uint[]
			{
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsname_2201,
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsrepositoryid_2202,
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsspdxid_2203,
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsspdxversion_2204,
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsdatalicense_2205,
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialscreatedatutc_2206,
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsdocumentnamespace_2207,
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialslastpolledatutc_2199,
			};
			object[] teams = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorysoftwarebillofmaterials.tablePid, organizationTeamsIdx);
			object[] name = (object[])teams[0];
			object[] repositoryId = (object[])teams[1];
			object[] spdxId = (object[])teams[2];
			object[] spdxVersion = (object[])teams[3];
			object[] dataLicense = (object[])teams[4];
			object[] createAt = (object[])teams[5];
			object[] documentNamespace = (object[])teams[6];
			object[] lastPolledAt = (object[])teams[7];

			for (int i = 0; i < name.Length; i++)
			{
				Rows.Add(new SoftwareBillOfMaterialsTableRow(
				name[i],
				repositoryId[i],
				spdxId[i],
				spdxVersion[i],
				dataLicense[i],
				createAt[i],
				documentNamespace[i],
				lastPolledAt[i]));
			}
		}

		public static event EventHandler<TableEventArgs> SoftwareBillOfMaterialsChanged;

		public List<SoftwareBillOfMaterialsTableRow> Rows { get; set; } = new List<SoftwareBillOfMaterialsTableRow>();

		public static SoftwareBillOfMaterialsTable GetTable(SLProtocol protocol = null)
		{
			if (protocol is null)
			{
				return instance;
			}

			instance?.Dispose();
			instance = new SoftwareBillOfMaterialsTable(protocol);
			return instance;
		}

		public void SaveToProtocol(SLProtocol protocol, bool partial = false)
		{
			// Calculate the batch size, recommended 25000 cells max per fill array, divided by the number of columns.
			var batchSize = 25000 / 8;

			// If full then the first batch needs to be a SaveOption.Full.
			var first = !partial;
			if (!Rows.Any() && !partial)
			{
				protocol.ClearAllKeys(Parameter.Repositorysoftwarebillofmaterials.tablePid);
				return;
			}

			foreach (var batch in Rows.Select(x => x.ToProtocolRow()).Batch(batchSize))
			{
				if (first)
				{
					protocol.FillArray(Parameter.Repositorysoftwarebillofmaterials.tablePid, batch.ToList(), NotifyProtocol.SaveOption.Full);
				}
				else
				{
					protocol.FillArray(Parameter.Repositorysoftwarebillofmaterials.tablePid, batch.ToList(), NotifyProtocol.SaveOption.Partial);
				}
			}
		}

		public void DeleteRow(SLProtocol protocol, params string[] rowsToDelete)
		{
			if (rowsToDelete.Length <= 0)
				return;

			// Remove from DataMiner and local instance
			protocol.DeleteRow(Parameter.Repositorysoftwarebillofmaterials.tablePid, rowsToDelete);
			instance.Rows.RemoveAll(x => rowsToDelete.ToHashSet().Contains(x.Name));
			SoftwareBillOfMaterialsChanged?.Invoke(instance, new TableEventArgs(protocol, TableChange.Remove, rowsToDelete));
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

		private void RepositoriesTable_RepositoriesChanged(object sender, RepositoryEventArgs e)
		{
			// There only needs to happen something when removing a repository
			if (e.Type != RepositoryChange.Remove)
				return;

			var idx = new uint[]
			{
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsname_2201,
				Parameter.Repositorysoftwarebillofmaterials.Idx.repositorysoftwarebillofmaterialsrepositoryid_2202,
			};

			var toBeRemoved = ((object[])e.Protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorysoftwarebillofmaterials.tablePid, idx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => e.Repositories.Contains(row[1]))
				.Select(row => row[0])
				.ToArray();

			DeleteRow(e.Protocol, toBeRemoved);
		}
	}
}
