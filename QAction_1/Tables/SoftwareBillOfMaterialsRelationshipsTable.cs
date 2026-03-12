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

	public class SoftwareBillOfMaterialsRelationshipsTableRow
	{
		private DateTime lastPolledAt;

		public SoftwareBillOfMaterialsRelationshipsTableRow() { }

		public SoftwareBillOfMaterialsRelationshipsTableRow(params object[] row)
		{
			Instance = Convert.ToString(row[0]);
			SoftwareBillOfMaterialsName = Convert.ToString(row[1]);
			RepositoryID = Convert.ToString(row[2]);
			SpdxElementID = Convert.ToString(row[3]);
			RelatedSpdxElementID = Convert.ToString(row[4]);
			RelationshipType = Convert.ToString(row[5]);
			LastPolledAt = DateTime.SpecifyKind(DateTime.FromOADate(Convert.ToDouble(row[6])), DateTimeKind.Utc);
		}

		public string Instance { get; set; } = Exceptions.NotAvailable;

		public string SoftwareBillOfMaterialsName { get; set; }

		public string RepositoryID { get; set; }

		public string SpdxElementID { get; set; }

		public string RelatedSpdxElementID { get; set; }

		public string RelationshipType { get; set; }

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

		public static SoftwareBillOfMaterialsRelationshipsTableRow FromPK(SLProtocol protocol, string pk)
		{
			var row = (object[])protocol.GetRow(Parameter.Repositorysoftwarebillofmaterialsrelationships.tablePid, pk);
			if (row[0] == null)
			{
				return default;
			}

			return new SoftwareBillOfMaterialsRelationshipsTableRow(row);
		}

		public object[] ToProtocolRow()
		{
			return new RepositorysoftwarebillofmaterialsrelationshipsQActionRow
			{
				Repositorysoftwarebillofmaterialsrelationshipsinstance_2601 = Instance,
				Repositorysoftwarebillofmaterialsrelationshipssbomid_2602 = SoftwareBillOfMaterialsName,
				Repositorysoftwarebillofmaterialsrelationshipsrepositoryid_2603 = RepositoryID,
				Repositorysoftwarebillofmaterialsrelationshipsspdxelementid_2604 = SpdxElementID,
				Repositorysoftwarebillofmaterialsrelationshipsrelatedspdxelementid_2605 = RelatedSpdxElementID,
				Repositorysoftwarebillofmaterialsrelationshipsrelationshiptype_2606 = RelationshipType,
				Repositorysoftwarebillofmaterialsrelationshipslastpolledatutc_2599 = LastPolledAt.ToOADate(),
			};
		}

		public void SaveToProtocol(SLProtocol protocol)
		{
			if (!protocol.Exists(Parameter.Repositorysoftwarebillofmaterialsrelationships.tablePid, Instance))
			{
				protocol.AddRow(Parameter.Repositorysoftwarebillofmaterialsrelationships.tablePid, ToProtocolRow());
			}
			else
			{
				protocol.SetRow(Parameter.Repositorysoftwarebillofmaterialsrelationships.tablePid, Instance, ToProtocolRow());
			}
		}
	}

	public class SoftwareBillOfMaterialsRelationshipsTable : IDisposable
	{
		private static SoftwareBillOfMaterialsRelationshipsTable instance = new SoftwareBillOfMaterialsRelationshipsTable();

		public SoftwareBillOfMaterialsRelationshipsTable()
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;
			SoftwareBillOfMaterialsTable.SoftwareBillOfMaterialsChanged += SoftwareBillOfMaterialsTable_SoftwareBillOfMaterialsChanged;
		}

		public SoftwareBillOfMaterialsRelationshipsTable(SLProtocol protocol)
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;
			SoftwareBillOfMaterialsTable.SoftwareBillOfMaterialsChanged += SoftwareBillOfMaterialsTable_SoftwareBillOfMaterialsChanged;

			uint[] organizationTeamsIdx = new uint[]
			{
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsinstance_2601,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipssbomid_2602,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsrepositoryid_2603,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsspdxelementid_2604,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsrelatedspdxelementid_2605,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsrelationshiptype_2606,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipslastpolledatutc_2599,
			};

			object[] teams = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorysoftwarebillofmaterialspackages.tablePid, organizationTeamsIdx);
			object[] pk = (object[])teams[0];
			object[] sbomId = (object[])teams[1];
			object[] repositoryId = (object[])teams[2];
			object[] spdxElementId = (object[])teams[3];
			object[] relatedeSpdxElementId = (object[])teams[4];
			object[] relationshipType = (object[])teams[5];
			object[] lastPolledAt = (object[])teams[6];

			for (int i = 0; i < pk.Length; i++)
			{
				Rows.Add(new SoftwareBillOfMaterialsRelationshipsTableRow(
				pk[i],
				sbomId[i],
				repositoryId[i],
				spdxElementId[i],
				relatedeSpdxElementId[i],
				relationshipType[i],
				lastPolledAt[i]));
			}
		}

		public static event EventHandler<TableEventArgs> SoftwareBillOfMaterialsPackagesChanged;

		public List<SoftwareBillOfMaterialsRelationshipsTableRow> Rows { get; set; } = new List<SoftwareBillOfMaterialsRelationshipsTableRow>();

		public static SoftwareBillOfMaterialsRelationshipsTable GetTable(SLProtocol protocol = null)
		{
			if (protocol is null)
			{
				return instance;
			}

			instance?.Dispose();
			instance = new SoftwareBillOfMaterialsRelationshipsTable(protocol);
			return instance;
		}

		public void SaveToProtocol(SLProtocol protocol, bool partial = false)
		{
			// Calculate the batch size, recommended 25000 cells max per fill array, divided by the number of columns.
			var batchSize = 25000 / 7;

			// If full then the first batch needs to be a SaveOption.Full.
			var first = !partial;
			if (!Rows.Any() && !partial)
			{
				protocol.ClearAllKeys(Parameter.Repositorysoftwarebillofmaterialsrelationships.tablePid);
				return;
			}

			foreach (var batch in Rows.Select(x => x.ToProtocolRow()).Batch(batchSize))
			{
				if (first)
				{
					protocol.FillArray(Parameter.Repositorysoftwarebillofmaterialsrelationships.tablePid, batch.ToList(), NotifyProtocol.SaveOption.Full);
				}
				else
				{
					protocol.FillArray(Parameter.Repositorysoftwarebillofmaterialsrelationships.tablePid, batch.ToList(), NotifyProtocol.SaveOption.Partial);
				}
			}
		}

		public void DeleteRow(SLProtocol protocol, params string[] rowsToDelete)
		{
			if (rowsToDelete.Length <= 0)
				return;

			// Remove from DataMiner and local instance
			protocol.DeleteRow(Parameter.Repositorysoftwarebillofmaterialsrelationships.tablePid, rowsToDelete);
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
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsinstance_2601,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsrepositoryid_2603,
			};

			var toBeRemoved = ((object[])e.Protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorysoftwarebillofmaterialsrelationships.tablePid, idx))
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
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipsinstance_2601,
				Parameter.Repositorysoftwarebillofmaterialsrelationships.Idx.repositorysoftwarebillofmaterialsrelationshipssbomid_2602,
			};

			var toBeRemoved = ((object[])e.Protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorysoftwarebillofmaterialsrelationships.tablePid, idx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => e.PrimaryKeys.Contains(row[1]))
				.Select(row => row[0])
				.ToArray();

			DeleteRow(e.Protocol, toBeRemoved);
		}
	}
}
