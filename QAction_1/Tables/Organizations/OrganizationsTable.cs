namespace Skyline.Protocol.Tables
{
	using System;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.Protocol.PollManager;

	public class OrganizationsRowConverter : ISLRowConverter<OrganizationsModel, OrganizationsQActionRow>
	{
		public static readonly OrganizationsRowConverter Instance = new OrganizationsRowConverter();

		private readonly ColumnMapBase<OrganizationsModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<OrganizationsModel>[] _columnWriteMaps;

		protected OrganizationsRowConverter()
		{
			_columnMaps = new ColumnMapBase<OrganizationsModel>[]
			{
				SLTables.Organizations.Instance.Read.Map<OrganizationsModel>(m => m.Instance),
				SLTables.Organizations.Id.Read.Map<OrganizationsModel>(m => m.Id),
				SLTables.Organizations.Description.Read.Map<OrganizationsModel>(m => m.Description),
				SLTables.Organizations.AvatarUrl.Read.Map<OrganizationsModel>(m => m.AvatarUrl),
				SLTables.Organizations.Tracked.Read.Map<OrganizationsModel>(m => m.Tracked),
				SLTables.Organizations.LastPolledAt.Read.Map<OrganizationsModel>(m => m.LastPolledAt),
			};
			_columnWriteMaps = new ColumnWriteMapBase<OrganizationsModel>[]
			{
				SLTables.Organizations.Instance.Read.MapWrite<OrganizationsModel>(m => m.Instance),
				SLTables.Organizations.Id.Read.MapWrite<OrganizationsModel>(m => m.Id),
				SLTables.Organizations.Description.Read.MapWrite<OrganizationsModel>(m => m.Description),
				SLTables.Organizations.AvatarUrl.Read.MapWrite<OrganizationsModel>(m => m.AvatarUrl),
				SLTables.Organizations.Tracked.Read.MapWrite<OrganizationsModel>(m => m.Tracked),
				SLTables.Organizations.LastPolledAt.Read.MapWrite<OrganizationsModel>(m => m.LastPolledAt),
			};
		}

		public OrganizationsModel FromRawValue(OrganizationsQActionRow rawRow)
		{
			var row = new OrganizationsModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public OrganizationsQActionRow ToRawValue(OrganizationsModel row)
		{
			var qactionRow = new OrganizationsQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new OrganizationsQActionRow(rawRow);
		}
	}

	public class OrganizationsModel
	{
		public string Instance { get; set; }

		public double? Id { get; set; }

		public string Description { get; set; }

		public string AvatarUrl { get; set; }

		public bool? Tracked { get; set; }

		public DateTime? LastPolledAt { get; set; }
	}

	public class OrganizationsQActionTable : SLTable<OrganizationsQActionRow>
	{
		public static readonly OrganizationsQActionTable Singleton = new OrganizationsQActionTable();

		protected OrganizationsQActionTable()
			: base(Parameter.Organizations.tablePid, Parameter.Organizations.indexColumn, Parameter.Organizations.indexColumnPid)
		{
			Instance = new SLReadColumn<string>(
				Parameter.Organizations.Idx.organizationsinstance_3001,
				Parameter.Organizations.Pid.organizationsinstance_3001,
				this);

			Id = new SLReadColumn<double?>(
				Parameter.Organizations.Idx.organizationsid_3002,
				Parameter.Organizations.Pid.organizationsid_3002,
				this);

			Description = new SLReadColumn<string>(
				Parameter.Organizations.Idx.organizationsdescription_3003,
				Parameter.Organizations.Pid.organizationsdescription_3003,
				this);

			AvatarUrl = new SLReadColumn<string>(
				Parameter.Organizations.Idx.organizationsavatarurl_3004,
				Parameter.Organizations.Pid.organizationsavatarurl_3004,
				this);

			Tracked = new SLReadWriteColumn<bool?, bool>(
				Parameter.Organizations.Idx.organizationstrackrepositories_3005,
				Parameter.Organizations.Pid.organizationstrackrepositories_3005,
				Parameter.Write.organizationstrackrepositories_3105,
				this);

			LastPolledAt = new SLReadColumn<DateTime?>(
				Parameter.Organizations.Idx.organizationslastpolledatutc_2999,
				Parameter.Organizations.Pid.organizationslastpolledatutc_2999,
				this);
		}

		public SLReadColumn<string> Instance { get; }

		public SLReadColumn<double?> Id { get; }

		public SLReadColumn<string> Description { get; }

		public SLReadColumn<string> AvatarUrl { get; }

		public SLReadWriteColumn<bool?, bool> Tracked { get; }

		public SLReadColumn<DateTime?> LastPolledAt { get; }

		public void Cleanup(SLProtocol protocol)
		{
			var pollRow = PollManagerRowConverter.Instance.FromRawValue(
				SLTables.PollManager.GetRow(protocol, Convert.ToString((int)RequestType.Organizations_User)));
			var pollTime = pollRow.PollingStatus == PollingStatus.Polling ? pollRow.PreviouslyPolledUTCTime : pollRow.LastPolledUTCTime;
			if (!pollTime.HasValue)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(OrganizationsQActionTable)}.{nameof(Cleanup)}|Table hasn't been polled yet", LogType.DebugInfo, LogLevel.Level2);
				return;
			}

			var toBeRemoved = SLTables.Organizations.GetData(
				protocol,
				Instance.Read.Map<OrganizationsModel>(m => m.Instance),
				LastPolledAt.Read.Map<OrganizationsModel>(m => m.LastPolledAt))
					.Where(m =>
						!m.LastPolledAt.HasValue ||
						(m.LastPolledAt < pollTime))
					.Select(m => m.Instance)
					.ToHashSet();

			DeleteRows(protocol, toBeRemoved);
		}

		protected override void DisposeEvents()
		{
		}
	}
}
