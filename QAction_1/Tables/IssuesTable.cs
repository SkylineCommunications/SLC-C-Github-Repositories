namespace Skyline.Protocol.Tables
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Skyline.DataMiner.Net;
    using Skyline.DataMiner.Scripting;
    using Skyline.Protocol.Extensions;
    using SLNetMessages = Skyline.DataMiner.Net.Messages;

    public enum IssueState
	{
		Closed,
		Open,
	}

	public class RepositoryIssuesRow
	{
		private string assignee;
		private DateTime createdAt;
		private double createdAtOA = Exceptions.IntNotAvailable;
		private DateTime updatedAt;
		private double updatedAtOA = Exceptions.IntNotAvailable;
		private DateTime closedAt;
		private double closedAtOA = Exceptions.IntNotAvailable;

		public RepositoryIssuesRow() { }

		public RepositoryIssuesRow(params object[] row)
		{
			Instance = Convert.ToString(row[0]);
			Number = Convert.ToInt32(row[1]);
			Title = Convert.ToString(row[2]);
			Body = Convert.ToString(row[3]);
			Creator = Convert.ToString(row[4]);
			State = String.IsNullOrEmpty(Convert.ToString(row[5])) ? IssueState.Open : (IssueState)Enum.Parse(typeof(IssueState), Convert.ToString(row[5]), true);
			Assignee = Convert.ToString(row[6]);
			CreatedAt = DateTime.FromOADate(Convert.ToDouble(row[7]));
			UpdatedAt = DateTime.FromOADate(Convert.ToDouble(row[8]));
			ClosedAt = DateTime.FromOADate(Convert.ToDouble(row[9]));
			RepositoryID = Convert.ToString(row[10]);
		}

		public string Instance { get; set; }

		public int Number { get; set; }

		public string Title { get; set; }

		public string Body { get; set; }

		public string Creator { get; set; }

		public IssueState State { get; set; }

		public string Assignee
		{
			get
			{
				return assignee;
			}

			set
			{
				if (value == null)
				{
					assignee = Exceptions.NotAvailable;
				}
				else
				{
					assignee = value;
				}
			}
		}

		public DateTime CreatedAt
		{
			get
			{
				return createdAt;
			}

			set
			{
				createdAt = value;
				if (value == default(DateTime))
				{
					createdAtOA = Exceptions.IntNotAvailable;
				}
				else
				{
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
				updatedAt = value;
				if (value == default(DateTime))
				{
					updatedAtOA = Exceptions.IntNotAvailable;
				}
				else
				{
					updatedAtOA = value.ToOADate();
				}
			}
		}

		public DateTime ClosedAt
		{
			get
			{
				return closedAt;
			}

			set
			{
				closedAt = value;
				if (value == default(DateTime))
				{
					closedAtOA = Exceptions.IntNotAvailable;
				}
				else
				{
					closedAtOA = value.ToOADate();
				}
			}
		}

		public string RepositoryID { get; set; }

		public static RepositoryIssuesRow FromPK(SLProtocol protocol, string pk)
		{
			var row = (object[])protocol.GetRow(Parameter.Repositoryissues.tablePid, pk);
			if (row[0] == null)
			{
				return default;
			}

			return new RepositoryIssuesRow(row);
		}

		public object[] ToProtocolRow()
		{
			return new RepositoryissuesQActionRow
			{
				Repositoryissuesinstance = Instance,
				Repositoryissuesnumber = Number,
				Repositoryissuestitle = Title,
				Repositoryissuesbody = Body,
				Repositoryissuescreator = Creator,
				Repositoryissuesstate = Enum.GetName(typeof(IssueState), State).ToLower(),
				Repositoryissuesassignee = Assignee,
				Repositoryissuescreatedat = createdAtOA,
				Repositoryissuesupdatedat = updatedAtOA,
				Repositoryissuesclosedat = closedAtOA,
				Repositoryissuesrepositoryid = RepositoryID,
			};
		}

		public void SaveToProtocol(SLProtocol protocol)
		{
			if (!protocol.Exists(Parameter.Repositoryissues.tablePid, Convert.ToString(Instance)))
			{
				protocol.AddRow(Parameter.Repositoryissues.tablePid, ToProtocolRow());
			}
			else
			{
				protocol.SetRow(Parameter.Repositoryissues.tablePid, Instance, ToProtocolRow());
			}
		}
	}

	public class RepositoryIssuesTable : IDisposable
	{
		private static RepositoryIssuesTable instance = new RepositoryIssuesTable();

		#region Constructor
		protected RepositoryIssuesTable()
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;
		}

		protected RepositoryIssuesTable(SLProtocol protocol)
		{
			RepositoriesTable.RepositoriesChanged += RepositoriesTable_RepositoriesChanged;

			uint[] repositoryIssuesIdx = new uint[]
			{
				Parameter.Repositoryissues.Idx.repositoryissuesinstance,
				Parameter.Repositoryissues.Idx.repositoryissuesnumber,
				Parameter.Repositoryissues.Idx.repositoryissuestitle,
				Parameter.Repositoryissues.Idx.repositoryissuesbody,
				Parameter.Repositoryissues.Idx.repositoryissuescreator,
				Parameter.Repositoryissues.Idx.repositoryissuesstate,
				Parameter.Repositoryissues.Idx.repositoryissuesassignee,
				Parameter.Repositoryissues.Idx.repositoryissuescreatedat,
				Parameter.Repositoryissues.Idx.repositoryissuesupdatedat,
				Parameter.Repositoryissues.Idx.repositoryissuesclosedat,
				Parameter.Repositoryissues.Idx.repositoryissuesrepositoryid,
			};
			object[] repositoryissues = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryissues.tablePid, repositoryIssuesIdx);
			object[] instance = (object[])repositoryissues[0];
			object[] number = (object[])repositoryissues[1];
			object[] title = (object[])repositoryissues[2];
			object[] body = (object[])repositoryissues[3];
			object[] creator = (object[])repositoryissues[4];
			object[] state = (object[])repositoryissues[5];
			object[] assignee = (object[])repositoryissues[6];
			object[] createdAt = (object[])repositoryissues[7];
			object[] updatedAt = (object[])repositoryissues[8];
			object[] closedAt = (object[])repositoryissues[9];
			object[] repositoryID = (object[])repositoryissues[10];

			for (int i = 0; i < instance.Length; i++)
			{
				Rows.Add(new RepositoryIssuesRow(
				instance[i],
				number[i],
				title[i],
				body[i],
				creator[i],
				state[i],
				assignee[i],
				createdAt[i],
				updatedAt[i],
				closedAt[i],
				repositoryID[i]));
			}
		}

		~RepositoryIssuesTable()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: false);
		}
		#endregion

		public List<RepositoryIssuesRow> Rows { get; private set; } = new List<RepositoryIssuesRow>();

		public static RepositoryIssuesTable GetTable(SLProtocol protocol = null)
		{
			if (protocol != null)
			{
				instance.Dispose();
				instance = new RepositoryIssuesTable(protocol);
			}

			return instance;
		}

		public void SaveToProtocol(SLProtocol protocol, bool partial = false)
		{
			List<object[]> rows = Rows.Select(x => x.ToProtocolRow()).ToList();
			NotifyProtocol.SaveOption option = partial ? NotifyProtocol.SaveOption.Partial : NotifyProtocol.SaveOption.Full;
			protocol.FillArray(Parameter.Repositoryissues.tablePid, rows, option);
		}

		#region IDisposable
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
			Dispose(disposing: true);
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

			// Delete Linked Issues
			var issuesIdx = new uint[]
			{
				Parameter.Repositoryissues.Idx.repositoryissuesinstance,
				Parameter.Repositoryissues.Idx.repositoryissuesrepositoryid,
			};

			var issuesRows = ((object[])e.Protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryissues.tablePid, issuesIdx))
				.Select(col => Array.ConvertAll((object[])col, Convert.ToString))
				.ToRows()
				.Where(row => e.Repositories.Contains(row[1]))
				.Select(row => row[0]);

			e.Protocol.DeleteRow(Parameter.Repositoryissues.tablePid, issuesRows.ToArray());
		}
	}
}
