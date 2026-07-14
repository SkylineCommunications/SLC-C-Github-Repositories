namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.DataMiner.Scripting.Helper.Events;
	using Skyline.Protocol.PollManager;

	public class TeamsRowConverter : ISLRowConverter<TeamsModel, OrganizationteamsQActionRow>
	{
		public static readonly TeamsRowConverter Instance = new TeamsRowConverter();

		private readonly ColumnMapBase<TeamsModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<TeamsModel>[] _columnWriteMaps;

		protected TeamsRowConverter()
		{
			_columnMaps = new ColumnMapBase<TeamsModel>[]
			{
				SLTables.Teams.Instance.Read.Map<TeamsModel>(m => m.Instance),
				SLTables.Teams.Id.Read.Map<TeamsModel>(m => m.Id),
				SLTables.Teams.Organization.Read.Map<TeamsModel>(m => m.Organization),
				SLTables.Teams.Name.Read.Map<TeamsModel>(m => m.Name),
				SLTables.Teams.Slug.Read.Map<TeamsModel>(m => m.Slug),
				SLTables.Teams.Description.Read.Map<TeamsModel>(m => m.Description),
				SLTables.Teams.Privacy.Read.Map<TeamsModel>(m => m.Privacy),
				SLTables.Teams.NotificationsEnabled.Read.Map<TeamsModel>(m => m.NotificationsEnabled),
				SLTables.Teams.Permission.Read.Map<TeamsModel>(m => m.Permission),
				SLTables.Teams.LastPolledAt.Read.Map<TeamsModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<TeamsModel>[]
			{
				SLTables.Teams.Instance.Read.MapWrite<TeamsModel>(m => m.Instance),
				SLTables.Teams.Id.Read.MapWrite<TeamsModel>(m => m.Id),
				SLTables.Teams.Organization.Read.MapWrite<TeamsModel>(m => m.Organization),
				SLTables.Teams.Name.Read.MapWrite<TeamsModel>(m => m.Name),
				SLTables.Teams.Slug.Read.MapWrite<TeamsModel>(m => m.Slug),
				SLTables.Teams.Description.Read.MapWrite<TeamsModel>(m => m.Description),
				SLTables.Teams.Privacy.Read.MapWrite<TeamsModel>(m => m.Privacy),
				SLTables.Teams.NotificationsEnabled.Read.MapWrite<TeamsModel>(m => m.NotificationsEnabled),
				SLTables.Teams.Permission.Read.MapWrite<TeamsModel>(m => m.Permission),
				SLTables.Teams.LastPolledAt.Read.MapWrite<TeamsModel>(m => m.LastPolledAt),
			};
		}

		public TeamsModel FromRawValue(OrganizationteamsQActionRow rawRow)
		{
			var row = new TeamsModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public OrganizationteamsQActionRow ToRawValue(TeamsModel row)
		{
			var qactionRow = new OrganizationteamsQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new OrganizationteamsQActionRow(rawRow);
		}
	}

	public class TeamsModel
	{
		public string Instance { get; set; }

		public long? Id { get; set; }

		public string Organization { get; set; }

		public string Name { get; set; }

		public string Slug { get; set; }

		public string Description { get; set; }

		public PrivacySetting? Privacy { get; set; }

		public NotificationSetting? NotificationsEnabled { get; set; }

		public string Permission { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class TeamsQActionTable : SLTable<OrganizationteamsQActionRow>
	{
		public static readonly TeamsQActionTable Singleton = new TeamsQActionTable();

		protected TeamsQActionTable()
			: base(Parameter.Organizationteams.tablePid, Parameter.Organizationteams.indexColumn, Parameter.Organizationteams.indexColumnPid)
		{
			PollManagerQActionTable.Singleton.StateChanged += PollManager_StateChanged;

			Instance = new SLReadColumn<string>(
				Parameter.Organizationteams.Idx.organizationteamsinstance,
				Parameter.Organizationteams.Pid.organizationteamsinstance,
				this);

			Id = new SLReadColumn<long?>(
				Parameter.Organizationteams.Idx.organizationteamsid,
				Parameter.Organizationteams.Pid.organizationteamsid,
				this);

			Organization = new SLReadColumn<string>(
				Parameter.Organizationteams.Idx.organizationteamsorganization,
				Parameter.Organizationteams.Pid.organizationteamsorganization,
				this);

			Name = new SLReadColumn<string>(
				Parameter.Organizationteams.Idx.organizationteamsname,
				Parameter.Organizationteams.Pid.organizationteamsname,
				this);

			Slug = new SLReadColumn<string>(
				Parameter.Organizationteams.Idx.organizationteamsslug,
				Parameter.Organizationteams.Pid.organizationteamsslug,
				this);

			Description = new SLReadColumn<string>(
				Parameter.Organizationteams.Idx.organizationteamsdescription,
				Parameter.Organizationteams.Pid.organizationteamsdescription,
				this);

			Privacy = new SLReadColumn<PrivacySetting?>(
				Parameter.Organizationteams.Idx.organizationteamsprivacy,
				Parameter.Organizationteams.Pid.organizationteamsprivacy,
				PrivacySettingConverter.Instance,
				this);

			NotificationsEnabled = new SLReadColumn<NotificationSetting?>(
				Parameter.Organizationteams.Idx.organizationteamsnotificationsenabled,
				Parameter.Organizationteams.Pid.organizationteamsnotificationsenabled,
				NotificationSettingConverter.Instance,
				this);

			Permission = new SLReadColumn<string>(
				Parameter.Organizationteams.Idx.organizationteamspermission,
				Parameter.Organizationteams.Pid.organizationteamspermission,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Organizationteams.Idx.organizationteamslastpolledatutc,
				Parameter.Organizationteams.Pid.organizationteamslastpolledatutc,
				this);
		}

		public SLReadColumn<string> Instance { get; }

		public SLReadColumn<long?> Id { get; }

		public SLReadColumn<string> Organization { get; }

		public SLReadColumn<string> Name { get; }

		public SLReadColumn<string> Slug { get; }

		public SLReadColumn<string> Description { get; }

		public SLReadColumn<PrivacySetting?> Privacy { get; }

		public SLReadColumn<NotificationSetting?> NotificationsEnabled { get; }

		public SLReadColumn<string> Permission { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol, string organization)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Organizations_Teams)));
			var toBeRemoved = SLTables.Teams.GetData(
				protocol,
				Instance.Read.Map<TeamsModel>(m => m.Instance),
				Organization.Read.Map<TeamsModel>(m => m.Organization),
				LastPolledAt.Read.Map<TeamsModel>(m => m.LastPolledAt))
					.Where(m => m.Organization == organization)
					.Where(m => m.LastPolledAt < pollRow.LastPolledUTCTime)
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		public void Cleanup(SLProtocol protocol)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Organizations_Teams)));
			var pollTime = pollRow.PollingStatus == PollingStatus.Polling ? pollRow.PreviouslyPolledUTCTime : pollRow.LastPolledUTCTime;
			if (!pollTime.HasValue)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(TeamsQActionTable)}.{nameof(Cleanup)}|Table hasn't been polled yet", LogType.DebugInfo, LogLevel.Level2);
				return;
			}

			var toBeRemoved = SLTables.Teams.GetData(
				protocol,
				Instance.Read.Map<TeamsModel>(m => m.Instance),
				LastPolledAt.Read.Map<TeamsModel>(m => m.LastPolledAt))
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
			if (e.RequestType != RequestType.Organizations_Teams)
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
