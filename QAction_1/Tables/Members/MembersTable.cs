namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.Protocol.PollManager;

	public class MembersRowConverter : ISLRowConverter<MembersModel, OrganizationmembersQActionRow>
	{
		public static readonly MembersRowConverter Instance = new MembersRowConverter();

		private readonly ColumnMapBase<MembersModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<MembersModel>[] _columnWriteMaps;

		protected MembersRowConverter()
		{
			_columnMaps = new ColumnMapBase<MembersModel>[]
			{
				SLTables.Members.Instance.Read.Map<MembersModel>(m => m.Instance),
				SLTables.Members.Id.Read.Map<MembersModel>(m => m.Id),
				SLTables.Members.Login.Read.Map<MembersModel>(m => m.Login),
				SLTables.Members.Type.Read.Map<MembersModel>(m => m.Type),
				SLTables.Members.SiteAdmin.Read.Map<MembersModel>(m => m.SiteAdmin),
				SLTables.Members.Url.Read.Map<MembersModel>(m => m.Url),
				SLTables.Members.HtmlUrl.Read.Map<MembersModel>(m => m.HtmlUrl),
				SLTables.Members.AvatarUrl.Read.Map<MembersModel>(m => m.AvatarUrl),
				SLTables.Members.LastPolledAt.Read.Map<MembersModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<MembersModel>[]
			{
				SLTables.Members.Instance.Read.MapWrite<MembersModel>(m => m.Instance),
				SLTables.Members.Id.Read.MapWrite<MembersModel>(m => m.Id),
				SLTables.Members.Login.Read.MapWrite<MembersModel>(m => m.Login),
				SLTables.Members.Type.Read.MapWrite<MembersModel>(m => m.Type),
				SLTables.Members.SiteAdmin.Read.MapWrite<MembersModel>(m => m.SiteAdmin),
				SLTables.Members.Url.Read.MapWrite<MembersModel>(m => m.Url),
				SLTables.Members.HtmlUrl.Read.MapWrite<MembersModel>(m => m.HtmlUrl),
				SLTables.Members.AvatarUrl.Read.MapWrite<MembersModel>(m => m.AvatarUrl),
				SLTables.Members.LastPolledAt.Read.MapWrite<MembersModel>(m => m.LastPolledAt),
			};
		}

		public MembersModel FromRawValue(OrganizationmembersQActionRow rawRow)
		{
			var row = new MembersModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public OrganizationmembersQActionRow ToRawValue(MembersModel row)
		{
			var qactionRow = new OrganizationmembersQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new OrganizationmembersQActionRow(rawRow);
		}
	}

	public class MembersModel
	{
		public string Instance { get; set; }

		public long? Id { get; set; }

		public string Login { get; set; }

		public string Type { get; set; }

		public bool? SiteAdmin { get; set; }

		public string Url { get; set; }

		public string HtmlUrl { get; set; }

		public string AvatarUrl { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class MembersQActionTable : SLTable<OrganizationmembersQActionRow>
	{
		public static readonly MembersQActionTable Singleton = new MembersQActionTable();

		protected MembersQActionTable()
			: base(Parameter.Organizationmembers.tablePid, Parameter.Organizationmembers.indexColumn, Parameter.Organizationmembers.indexColumnPid)
		{
			PollManagerQActionTable.Singleton.StateChanged += PollManager_StateChanged;

			Instance = new SLReadColumn<string>(
				Parameter.Organizationmembers.Idx.organizationmembersinstance_3601,
				Parameter.Organizationmembers.Pid.organizationmembersinstance_3601,
				this);

			Id = new SLReadColumn<long?>(
				Parameter.Organizationmembers.Idx.organizationmembersid_3602,
				Parameter.Organizationmembers.Pid.organizationmembersid_3602,
				this);

			Login = new SLReadColumn<string>(
				Parameter.Organizationmembers.Idx.organizationmemberslogin_3603,
				Parameter.Organizationmembers.Pid.organizationmemberslogin_3603,
				this);

			Type = new SLReadColumn<string>(
				Parameter.Organizationmembers.Idx.organizationmemberstype_3604,
				Parameter.Organizationmembers.Pid.organizationmemberstype_3604,
				this);

			SiteAdmin = new SLReadColumn<bool?>(
				Parameter.Organizationmembers.Idx.organizationmemberssiteadmin_3605,
				Parameter.Organizationmembers.Pid.organizationmemberssiteadmin_3605,
				this);

			Url = new SLReadColumn<string>(
				Parameter.Organizationmembers.Idx.organizationmembersurl_3606,
				Parameter.Organizationmembers.Pid.organizationmembersurl_3606,
				this);

			HtmlUrl = new SLReadColumn<string>(
				Parameter.Organizationmembers.Idx.organizationmembershtmlurl_3607,
				Parameter.Organizationmembers.Pid.organizationmembershtmlurl_3607,
				this);

			AvatarUrl = new SLReadColumn<string>(
				Parameter.Organizationmembers.Idx.organizationmembersavatarurl_3608,
				Parameter.Organizationmembers.Pid.organizationmembersavatarurl_3608,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Organizationmembers.Idx.organizationmemberslastpolledatutc_3599,
				Parameter.Organizationmembers.Pid.organizationmemberslastpolledatutc_3599,
				this);
		}

		public SLReadColumn<string> Instance { get; }

		public SLReadColumn<long?> Id { get; }

		public SLReadColumn<string> Login { get; }

		public SLReadColumn<string> Type { get; }

		public SLReadColumn<bool?> SiteAdmin { get; }

		public SLReadColumn<string> Url { get; }

		public SLReadColumn<string> HtmlUrl { get; }

		public SLReadColumn<string> AvatarUrl { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Organizations_Members)));
			if (!pollRow.LastPolledUTCTime.HasValue)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(MembersQActionTable)}.{nameof(Cleanup)}|Table hasn't been polled yet", LogType.DebugInfo, LogLevel.Level2);
				return;
			}

			var toBeRemoved = SLTables.Members.GetData(
				protocol,
				Instance.Read.Map<MembersModel>(m => m.Instance),
				LastPolledAt.Read.Map<MembersModel>(m => m.LastPolledAt))
					.Where(m =>
						!m.LastPolledAt.HasValue ||
						(m.LastPolledAt < pollRow.LastPolledUTCTime))
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		protected override void DisposeEvents()
		{
			PollManagerQActionTable.Singleton.StateChanged -= PollManager_StateChanged;
		}

		private void PollManager_StateChanged(object sender, Poll_Manager.PollManagerStateChangedEventArgs e)
		{
			if (e.RequestType != RequestType.Organizations_Members)
			{
				return;
			}

			if (e.PollState != PollState.Disabled)
			{
				return;
			}

			ClearAllKeys(e.Protocol);
		}
	}
}
