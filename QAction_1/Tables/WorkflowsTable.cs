namespace Skyline.Protocol.Tables
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Scripting;

	using SLNetMessages = Skyline.DataMiner.Net.Messages;

	public class RepositoryWorkflowsRecord
	{
		private DateTime createdAt;
		private double createdAtOA = Exceptions.IntNotAvailable;
		private DateTime updatedAt;
		private double updatedAtOA = Exceptions.IntNotAvailable;
		private DateTime deletedAt;
		private double deletedAtOA = Exceptions.IntNotAvailable;

		public RepositoryWorkflowsRecord() { }

		public RepositoryWorkflowsRecord(params object[] row)
		{
			ID = Convert.ToString(row[0]);
			RepositoryID = Convert.ToString(row[1]);
			Name = Convert.ToString(row[2]);
			State = Convert.ToString(row[3]);
			Path = Convert.ToString(row[4]);
			CreatedAt = DateTime.FromOADate(Convert.ToDouble(row[5]));
			UpdatedAt = DateTime.FromOADate(Convert.ToDouble(row[6]));
			DeletedAt = DateTime.FromOADate(Convert.ToDouble(row[7]));
		}

		public string ID { get; set; }

		public string RepositoryID { get; set; }

		public string Name { get; set; }

		public string State { get; set; }

		public string Path { get; set; }

		public DateTime CreatedAt
		{
			get
			{
				return createdAt;
			}

			set
			{
				if (value == default)
				{
					createdAt = value;
					createdAtOA = Exceptions.IntNotAvailable;
				}
				else
				{
					createdAt = value;
					createdAtOA = value.ToOADate();
				}
			}
		}

		public DateTime UpdatedAt
		{
			get
			{
				return updatedAt;
			}

			set
			{
				if (value == default)
				{
					updatedAt = value;
					updatedAtOA = Exceptions.IntNotAvailable;
				}
				else
				{
					updatedAt = value;
					updatedAtOA = value.ToOADate();
				}
			}
		}

		public DateTime DeletedAt
		{
			get
			{
				return deletedAt;
			}

			set
			{
				if(value == default)
				{
					deletedAt = value;
					deletedAtOA = Exceptions.IntNotAvailable;
				}
				else
				{
					deletedAt = value;
					deletedAtOA = value.ToOADate();
				}
			}
		}

		public static RepositoryWorkflowsRecord FromPK(SLProtocol protocol, string pk)
		{
			var row = (object[])protocol.GetRow(Parameter.Repositoryworkflows.tablePid, pk);
			if (row[0] == null)
			{
				return default;
			}

			return new RepositoryWorkflowsRecord(row);
		}

		public object[] ToProtocolRow()
		{
			return new RepositoryworkflowsQActionRow
			{
				Repositoryworkflowsid_1601 = ID,
				Repositoryworkflowsrepositoryid_1602 = RepositoryID,
				Repositoryworkflowsname_1603 = Name,
				Repositoryworkflowsstate_1604 = State,
				Repositoryworkflowspath_1605 = Path,
				Repositoryworkflowscreatedat_1606 = createdAtOA,
				Repositoryworkflowsupdatedat_1607 = updatedAtOA,
				Repositoryworkflowsdeletedat_1608 = deletedAtOA,
			};
		}

		public void SaveToProtocol(SLProtocol protocol)
		{
			if (!protocol.Exists(Parameter.Repositoryworkflows.tablePid, ID))
			{
				protocol.AddRow(Parameter.Repositoryworkflows.tablePid, ToProtocolRow());
			}
			else
			{
				protocol.SetRow(Parameter.Repositoryworkflows.tablePid, ID, ToProtocolRow());
			}
		}
	}

	public class RepositoryWorkflowsRecords
	{
		public RepositoryWorkflowsRecords() { }

		public RepositoryWorkflowsRecords(SLProtocol protocol)
		{
			uint[] repositoryWorkflowsIdx = new uint[]
			{
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsid_1601,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsrepositoryid_1602,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsname_1603,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsstate_1604,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowspath_1605,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowscreatedat_1606,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsupdatedat_1607,
				Parameter.Repositoryworkflows.Idx.repositoryworkflowsdeletedat_1608,
			};
			object[] repositoryworkflows = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryworkflows.tablePid, repositoryWorkflowsIdx);
			object[] iD = (object[])repositoryworkflows[0];
			object[] repositoryID = (object[])repositoryworkflows[1];
			object[] name = (object[])repositoryworkflows[2];
			object[] state = (object[])repositoryworkflows[3];
			object[] path = (object[])repositoryworkflows[4];
			object[] createdAt = (object[])repositoryworkflows[5];
			object[] updatedAt = (object[])repositoryworkflows[6];
			object[] deletedAt = (object[])repositoryworkflows[7];

			for (int i = 0; i < iD.Length; i++)
			{
				Rows.Add(new RepositoryWorkflowsRecord(
				iD[i],
				repositoryID[i],
				name[i],
				state[i],
				path[i],
				createdAt[i],
				updatedAt[i],
				deletedAt[i]));
			}
		}

		public List<RepositoryWorkflowsRecord> Rows { get; set; } = new List<RepositoryWorkflowsRecord>();

		public void SaveToProtocol(SLProtocol protocol, bool partial = false)
		{
			List<object[]> rows = Rows.Select(x => x.ToProtocolRow()).ToList();
			NotifyProtocol.SaveOption option = partial ? NotifyProtocol.SaveOption.Partial : NotifyProtocol.SaveOption.Full;
			protocol.FillArray(Parameter.Repositoryworkflows.tablePid, rows, option);
		}
	}
}
