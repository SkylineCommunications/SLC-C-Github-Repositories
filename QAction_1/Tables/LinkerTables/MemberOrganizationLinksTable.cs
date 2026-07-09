namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.DataMiner.Scripting.Helper.Events;
	using Skyline.Protocol.PollManager;

	public class MemberOrganizationLinksRowConverter : ISLRowConverter<MemberOrganizationLinksModel, MemberorganizationlinksQActionRow>
	{
		public static readonly MemberOrganizationLinksRowConverter Instance = new MemberOrganizationLinksRowConverter();

		private readonly ColumnMapBase<MemberOrganizationLinksModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<MemberOrganizationLinksModel>[] _columnWriteMaps;

		protected MemberOrganizationLinksRowConverter()
		{
			_columnMaps = new ColumnMapBase<MemberOrganizationLinksModel>[]
			{
				SLTables.MemberOrganizationLinks.Instance.Read.Map<MemberOrganizationLinksModel>(m => m.Instance),
				SLTables.MemberOrganizationLinks.Organization.Read.Map<MemberOrganizationLinksModel>(m => m.Organization),
				SLTables.MemberOrganizationLinks.Member.Read.Map<MemberOrganizationLinksModel>(m => m.Member),
				SLTables.MemberOrganizationLinks.LastPolledAt.Read.Map<MemberOrganizationLinksModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<MemberOrganizationLinksModel>[]
			{
				SLTables.MemberOrganizationLinks.Instance.Read.MapWrite<MemberOrganizationLinksModel>(m => m.Instance),
				SLTables.MemberOrganizationLinks.Organization.Read.MapWrite<MemberOrganizationLinksModel>(m => m.Organization),
				SLTables.MemberOrganizationLinks.Member.Read.MapWrite<MemberOrganizationLinksModel>(m => m.Member),
				SLTables.MemberOrganizationLinks.LastPolledAt.Read.MapWrite<MemberOrganizationLinksModel>(m => m.LastPolledAt),
			};
		}

		public MemberOrganizationLinksModel FromRawValue(MemberorganizationlinksQActionRow rawRow)
		{
			var row = new MemberOrganizationLinksModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public MemberorganizationlinksQActionRow ToRawValue(MemberOrganizationLinksModel row)
		{
			var qactionRow = new MemberorganizationlinksQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new MemberorganizationlinksQActionRow(rawRow);
		}
	}

	public class MemberOrganizationLinksModel
	{
		public string Instance { get; set; }

		public string Organization { get; set; }

		public string Member { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class MemberOrganizationLinksQActionTable : SLTable<MemberorganizationlinksQActionRow>
	{
		public static readonly MemberOrganizationLinksQActionTable Singleton = new MemberOrganizationLinksQActionTable();

		protected MemberOrganizationLinksQActionTable()
			: base(Parameter.Memberorganizationlinks.tablePid, Parameter.Memberorganizationlinks.indexColumn, Parameter.Memberorganizationlinks.indexColumnPid)
		{
			PollManagerQActionTable.Singleton.StateChanged += PollManager_StateChanged;

			Instance = new SLReadColumn<string>(
				Parameter.Memberorganizationlinks.Idx.memberorganizationlinksinstance_22001,
				Parameter.Memberorganizationlinks.Pid.memberorganizationlinksinstance_22001,
				this);

			Organization = new SLReadColumn<string>(
				Parameter.Memberorganizationlinks.Idx.memberorganizationlinksorganization_22002,
				Parameter.Memberorganizationlinks.Pid.memberorganizationlinksorganization_22002,
				this);

			Member = new SLReadColumn<string>(
				Parameter.Memberorganizationlinks.Idx.memberorganizationlinksmember_22003,
				Parameter.Memberorganizationlinks.Pid.memberorganizationlinksmember_22003,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Memberorganizationlinks.Idx.memberorganizationlinkslastpolledatutc_21999,
				Parameter.Memberorganizationlinks.Pid.memberorganizationlinkslastpolledatutc_21999,
				this);
		}

		public SLReadColumn<string> Instance { get; }

		public SLReadColumn<string> Organization { get; }

		public SLReadColumn<string> Member { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol, string organization)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Organizations_Members)));
			var toBeRemoved = SLTables.MemberOrganizationLinks.GetData(
				protocol,
				Instance.Read.Map<MemberOrganizationLinksModel>(m => m.Instance),
				Organization.Read.Map<MemberOrganizationLinksModel>(m => m.Organization),
				LastPolledAt.Read.Map<MemberOrganizationLinksModel>(m => m.LastPolledAt))
					.Where(m => m.Organization == organization)
					.Where(m => m.LastPolledAt < pollRow.LastPolledUTCTime)
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		public void Cleanup(SLProtocol protocol)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Organizations_Members)));
			var pollTime = pollRow.PollingStatus == PollingStatus.Polling ? pollRow.PreviouslyPolledUTCTime : pollRow.LastPolledUTCTime;
			if (!pollTime.HasValue)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(MembersQActionTable)}.{nameof(Cleanup)}|Table hasn't been polled yet", LogType.DebugInfo, LogLevel.Level2);
				return;
			}

			var toBeRemoved = SLTables.MemberOrganizationLinks.GetData(
				protocol,
				Instance.Read.Map<MemberOrganizationLinksModel>(m => m.Instance),
				LastPolledAt.Read.Map<MemberOrganizationLinksModel>(m => m.LastPolledAt))
					.Where(m =>
						!m.LastPolledAt.HasValue ||
						(m.LastPolledAt < pollTime))
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
