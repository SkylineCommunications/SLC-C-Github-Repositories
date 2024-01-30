namespace Skyline.Protocol.Tables
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;

	using SLNetMessages = Skyline.DataMiner.Net.Messages;

	public enum RepositoryType
	{
		Other = 0,
		Automation = 1,
		Connector = 2,
	}

	public class RepositoriesTableRow
	{
		private string description = Exceptions.NotAvailable;
		private double isPrivate = Exceptions.IntNotAvailable;
		private double isFork = Exceptions.IntNotAvailable;
		private DateTime createdAt;
		private double createdAtOA = Exceptions.IntNotAvailable;
		private DateTime updatedAt;
		private double updatedAtOA = Exceptions.IntNotAvailable;
		private DateTime pushedAt;
		private double pushedAtOA = Exceptions.IntNotAvailable;
		private string language = Exceptions.NotAvailable;

		public RepositoriesTableRow() { }

		public RepositoriesTableRow(params object[] row)
		{
			FullName = Convert.ToString(row[0]);
			Name = Convert.ToString(row[1]);
			Private = Convert.ToBoolean(row[2]);
			description = Convert.ToString(row[3]);
			Owner = Convert.ToString(row[4]);
			Fork = Convert.ToBoolean(row[5]);
			CreatedAt = DateTime.FromOADate(Convert.ToDouble(row[6]));
			UpdatedAt = DateTime.FromOADate(Convert.ToDouble(row[7]));
			PushedAt = DateTime.FromOADate(Convert.ToDouble(row[8]));
			Size = Convert.ToInt64(row[9]);
			Stars = Convert.ToInt32(row[10]);
			Watcher = Convert.ToInt32(row[11]);
			language = Convert.ToString(row[12]);
			DefaultBranch = Convert.ToString(row[13]);
			Type = (RepositoryType)Convert.ToInt16(row[14]);
			PublicKeyID = Convert.ToString(row[15]);
			PublicKey = Convert.ToString(row[16]);
		}

		public string Name { get; set; }

		public string FullName { get; set; } = Exceptions.NotAvailable;

		public bool Private { get => Convert.ToBoolean(isPrivate); set => isPrivate = Convert.ToDouble(value); }

		public string Description
		{
			get
			{
				if (description == Exceptions.NotAvailable)
				{
					return String.Empty;
				}
				else
				{
					return description;
				}
			}

			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					description = Exceptions.NotAvailable;
				}
				else
				{
					description = value;
				}
			}
		}

		public string Owner { get; set; } = Exceptions.NotAvailable;

		public bool Fork { get => Convert.ToBoolean(isFork); set => isFork = Convert.ToDouble(value); }

		public DateTime CreatedAt
		{
			get
			{
				return createdAt;
			}

			set
			{
				createdAt = value;
				createdAtOA = value.ToOADate();
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
				updatedAt = value;
				updatedAtOA = value.ToOADate();
			}
		}

		public DateTime PushedAt
		{
			get
			{
				return pushedAt;
			}

			set
			{
				pushedAt = value;
				pushedAtOA = value.ToOADate();
			}
		}

		public long Size { get; set; } = Exceptions.IntNotAvailable;

		public int Stars { get; set; } = Exceptions.IntNotAvailable;

		public int Watcher { get; set; } = Exceptions.IntNotAvailable;

		public string Language
		{
			get
			{
				if (language == Exceptions.NotAvailable)
				{
					return String.Empty;
				}
				else
				{
					return language;
				}
			}

			set
			{
				if (String.IsNullOrWhiteSpace(value))
				{
					language = Exceptions.NotAvailable;
				}
				else
				{
					language = value;
				}
			}
		}

		public string DefaultBranch { get; set; } = Exceptions.NotAvailable;

		public RepositoryType Type { get; set; } = RepositoryType.Other;

		public string PublicKeyID { get; set; } = Exceptions.NotAvailable;

		public string PublicKey { get; set; } = Exceptions.NotAvailable;

		public static RepositoryType GetTypeFromTopics(IEnumerable<string> topics)
		{
			if (topics.Contains("dataminer-automation-script"))
			{
				return RepositoryType.Automation;
			}
			else if (topics.Contains("dataminer-connector"))
			{
				return RepositoryType.Connector;
			}
			else
			{
				return RepositoryType.Other;
			}
		}

		public static RepositoriesTableRow FromPK(SLProtocol protocol, string pk)
		{
			var row = (object[])protocol.GetRow(Parameter.Repositories.tablePid, pk);
			if (row[0] == null)
			{
				return default;
			}

			return new RepositoriesTableRow(row);
		}

		public object[] ToProtocolRow()
		{
			return new RepositoriesQActionRow
			{
				Repositoriesname = Name,
				Repositoriesfullname = FullName,
				Repositoriesprivate = isPrivate,
				Repositoriesdescription = description,
				Repositoriesowner = Owner,
				Repositoriesfork = isFork,
				Repositoriescreatedat = createdAtOA,
				Repositoriesupdatedat = updatedAtOA,
				Repositoriespushedat = pushedAtOA,
				Repositoriessize = Convert.ToDouble(Size),
				Repositoriesstars = Convert.ToDouble(Stars),
				Repositorieswatcher = Convert.ToDouble(Watcher),
				Repositorieslanguage = language,
				Repositoriesdefaultbranch = DefaultBranch,
				Repositoriestype = Convert.ToInt16(Type),
				Repositoriespublickeyid = PublicKeyID,
				Repositoriespublickey = PublicKey,
			};
		}

		public void SaveToProtocol(SLProtocol protocol)
		{
			if (!protocol.Exists(Parameter.Repositories.tablePid, FullName))
			{
				protocol.AddRow(Parameter.Repositories.tablePid, ToProtocolRow());
			}
			else
			{
				protocol.SetRow(Parameter.Repositories.tablePid, FullName, ToProtocolRow());
			}
		}
	}

	public class RepositoriesTable
	{
		private static RepositoriesTable instance = new RepositoriesTable();

		#region Constructor
		protected RepositoriesTable() { }

		protected RepositoriesTable(SLProtocol protocol)
		{
			uint[] repositoriesTableIdx = new uint[]
			{
				Parameter.Repositories.Idx.repositoriesfullname,
				Parameter.Repositories.Idx.repositoriesname,
				Parameter.Repositories.Idx.repositoriesprivate,
				Parameter.Repositories.Idx.repositoriesdescription,
				Parameter.Repositories.Idx.repositoriesowner,
				Parameter.Repositories.Idx.repositoriesfork,
				Parameter.Repositories.Idx.repositoriescreatedat,
				Parameter.Repositories.Idx.repositoriesupdatedat,
				Parameter.Repositories.Idx.repositoriespushedat,
				Parameter.Repositories.Idx.repositoriessize,
				Parameter.Repositories.Idx.repositoriesstars,
				Parameter.Repositories.Idx.repositorieswatcher,
				Parameter.Repositories.Idx.repositorieslanguage,
				Parameter.Repositories.Idx.repositoriesdefaultbranch,
				Parameter.Repositories.Idx.repositoriestype,
				Parameter.Repositories.Idx.repositoriespublickeyid,
				Parameter.Repositories.Idx.repositoriespublickey,
			};
			object[] repositoriestable = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositories.tablePid, repositoriesTableIdx);
			object[] fullName = (object[])repositoriestable[0];
			object[] name = (object[])repositoriestable[1];
			object[] isprivate = (object[])repositoriestable[2];
			object[] description = (object[])repositoriestable[3];
			object[] owner = (object[])repositoriestable[4];
			object[] fork = (object[])repositoriestable[5];
			object[] createdAt = (object[])repositoriestable[6];
			object[] updatedAt = (object[])repositoriestable[7];
			object[] pushedAt = (object[])repositoriestable[8];
			object[] size = (object[])repositoriestable[9];
			object[] stars = (object[])repositoriestable[10];
			object[] watcher = (object[])repositoriestable[11];
			object[] language = (object[])repositoriestable[12];
			object[] defaultBranch = (object[])repositoriestable[13];
			object[] type = (object[])repositoriestable[14];
			object[] publicKeyId = (object[])repositoriestable[15];
			object[] publicKey = (object[])repositoriestable[16];

			for (int i = 0; i < name.Length; i++)
			{
				Rows.Add(new RepositoriesTableRow(
				fullName[i],
				name[i],
				isprivate[i],
				description[i],
				owner[i],
				fork[i],
				createdAt[i],
				updatedAt[i],
				pushedAt[i],
				size[i],
				stars[i],
				watcher[i],
				language[i],
				defaultBranch[i],
				type[i],
				publicKeyId[i],
				publicKey[i]));
			}
		}
		#endregion

		#region Events
		public static event EventHandler<RepositoryEventArgs> RepositoriesChanged;
		#endregion

		public List<RepositoriesTableRow> Rows { get; private set; } = new List<RepositoriesTableRow>();

		public static RepositoriesTable GetTable(SLProtocol protocol = null)
		{
			if (protocol != null)
			{
				instance = new RepositoriesTable(protocol);
			}

			return instance;
		}

		public void DeleteRow(SLProtocol protocol, params string[] rowsToDelete)
		{
			protocol.DeleteRow(Parameter.Repositories.tablePid, rowsToDelete);
			RepositoriesChanged?.Invoke(instance, new RepositoryEventArgs(protocol, RepositoryChange.Remove, rowsToDelete));
		}

		public void SaveToProtocol(SLProtocol protocol, bool partial = false)
		{
			List<object[]> rows = Rows.Select(x => x.ToProtocolRow()).ToList();
			NotifyProtocol.SaveOption option = partial ? NotifyProtocol.SaveOption.Partial : NotifyProtocol.SaveOption.Full;
			protocol.FillArray(Parameter.Repositories.tablePid, rows, option);

			protocol.SetParameter(Parameter.addworkflowrepository_discreetlist, String.Join(";", Rows.Select(row => row.FullName)));
		}
	}
}
