namespace Skyline.Protocol.Tables
{
	using System;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.Protocol.PollManager;
	using Skyline.Protocol.Tables.Poll_Manager;

	public class PollManagerRowConverter : ISLRowConverter<PollManagerModel, PollmanagerQActionRow>
	{
		public static readonly PollManagerRowConverter Instance = new PollManagerRowConverter();

		private readonly ColumnMapBase<PollManagerModel>[] _columnMaps;
		private readonly ColumnWriteMapBase<PollManagerModel>[] _columnWriteMaps;

		protected PollManagerRowConverter()
		{
			_columnMaps = new ColumnMapBase<PollManagerModel>[]
			{
				SLTables.PollManager.Index.Read.Map<PollManagerModel>(m => m.RequestType),
				SLTables.PollManager.Name.Read.Map<PollManagerModel>(m => m.Name),
				SLTables.PollManager.PollState.Read.Map<PollManagerModel>(m => m.PollState),
				SLTables.PollManager.PollFrequency.Read.Map<PollManagerModel>(m => m.PollFrequency),
				SLTables.PollManager.LastPolled.Read.Map<PollManagerModel>(m => m.LastPolledUTCTime),
				SLTables.PollManager.PreviouslyPolled.Read.Map<PollManagerModel>(m => m.PreviouslyPolledUTCTime),
				SLTables.PollManager.PageLimit.Read.Map<PollManagerModel>(m => m.PageLimit),
			};
			_columnWriteMaps = new ColumnWriteMapBase<PollManagerModel>[]
			{
				SLTables.PollManager.Index.Read.MapWrite<PollManagerModel>(m => m.RequestType),
				SLTables.PollManager.Name.Read.MapWrite<PollManagerModel>(m => m.Name),
				SLTables.PollManager.PollState.Read.MapWrite<PollManagerModel>(m => m.PollState),
				SLTables.PollManager.PollFrequency.Read.MapWrite<PollManagerModel>(m => m.PollFrequency),
				SLTables.PollManager.LastPolled.Read.MapWrite<PollManagerModel>(m => m.LastPolledUTCTime),
				SLTables.PollManager.PreviouslyPolled.Read.MapWrite<PollManagerModel>(m => m.PreviouslyPolledUTCTime),
				SLTables.PollManager.PageLimit.Read.MapWrite<PollManagerModel>(m => m.PageLimit),
			};
		}

		public PollManagerModel FromRawValue(PollmanagerQActionRow rawRow)
		{
			var row = new PollManagerModel();
			var objectRow = rawRow.ToObjectArray();
			foreach (var map in _columnMaps)
			{
				map.Apply(row, objectRow[map.Column.ColumnIndex]);
			}

			return row;
		}

		public PollmanagerQActionRow ToRawValue(PollManagerModel row)
		{
			var qactionRow = new PollmanagerQActionRow();
			var rawRow = new object[qactionRow.ColumnCount];
			foreach (var map in _columnWriteMaps)
			{
				rawRow[map.Column.ColumnIndex] = map.GetRaw(row);
			}

			return new PollmanagerQActionRow(rawRow);
		}
	}

	public class PollManagerModel
	{
		public RequestType? RequestType { get; set; }

		public string Name { get; set; }

		public PollState? PollState { get; set; }

		public TimeSpan? PollFrequency { get; set; }

		public DateTime? LastPolledUTCTime { get; set; }

		public DateTime? PreviouslyPolledUTCTime { get; set; }

		public int PageLimit { get; set; }
	}

	public class PollManagerQActionTable : SLTable<PollmanagerQActionRow>
	{
		public static readonly PollManagerQActionTable Singleton = new PollManagerQActionTable();

		protected PollManagerQActionTable()
			: base(Parameter.Pollmanager.tablePid, Parameter.Pollmanager.indexColumn, Parameter.Pollmanager.indexColumnPid)
		{
			Index = new SLReadColumn<RequestType?>(
				Parameter.Pollmanager.Idx.pollmanagerindex_21001,
				Parameter.Pollmanager.Pid.pollmanagerindex_21001,
				new RequestTypeConverter(),
				this);

			Name = new SLReadColumn<string>(
				Parameter.Pollmanager.Idx.pollmanagername_21002,
				Parameter.Pollmanager.Pid.pollmanagername_21002,
				this);

			PollState = new SLReadColumn<PollState?>(
				Parameter.Pollmanager.Idx.pollmanagerpollstate_21003,
				Parameter.Pollmanager.Pid.pollmanagerpollstate_21003,
				new PollStateConverter(),
				this);

			PollFrequency = new SLReadColumn<TimeSpan?>(
				Parameter.Pollmanager.Idx.pollmanagerpollfrequency_21004,
				Parameter.Pollmanager.Pid.pollmanagerpollfrequency_21004,
				this);

			LastPolled = new SLReadColumn<DateTime?>(
				Parameter.Pollmanager.Idx.pollmanagerlastpolled_21005,
				Parameter.Pollmanager.Pid.pollmanagerlastpolled_21005,
				this);

			PreviouslyPolled = new SLReadColumn<DateTime?>(
				Parameter.Pollmanager.Idx.pollmanagerpreviouslypolled_21007,
				Parameter.Pollmanager.Pid.pollmanagerpreviouslypolled_21007,
				new PreviouslyPolledConverter(),
				this);

			PageLimit = new SLReadColumn<int?>(
				Parameter.Pollmanager.Idx.pollmanagerpagelimit_21008,
				Parameter.Pollmanager.Pid.pollmanagerpagelimit_21008,
				this);
		}

		public event EventHandler<PollManagerStateChangedEventArgs> StateChanged;

		public SLReadColumn<RequestType?> Index { get; }

		public SLReadColumn<string> Name { get; }

		public SLReadColumn<PollState?> PollState { get; }

		public SLReadColumn<TimeSpan?> PollFrequency { get; }

		public SLReadColumn<DateTime?> LastPolled { get; }

		public SLReadColumn<DateTime?> PreviouslyPolled { get; }

		public SLReadColumn<int?> PageLimit { get; }

		public PollManagerModel GetRowByRequestType(SLProtocol protocol, RequestType requestType)
		{
			var row = GetRow(protocol, Convert.ToString((int)requestType));
			if (row == default)
			{
				return default;
			}

			return PollManagerRowConverter.Instance.FromRawValue(row);
		}

		public void TogglePollState(SLProtocol protocol, RequestType requestType)
		{
			var original = PollState.Read.GetCell(protocol, Convert.ToString((int)requestType));
			var newState = default(Protocol.PollManager.PollState);
			switch (original.Value)
			{
				case Protocol.PollManager.PollState.Disabled:
					newState = Protocol.PollManager.PollState.Enabled;
					break;

				case Protocol.PollManager.PollState.Enabled:
					newState = Protocol.PollManager.PollState.Disabled;
					break;
			}

			PollState.Read.SetCell(protocol, Convert.ToString((int)requestType), newState);
			StateChanged?.Invoke(this, new PollManagerStateChangedEventArgs(protocol, requestType, newState));
		}

		public void SetPollState(SLProtocol protocol, RequestType requestType, PollState pollState)
		{
			var original = PollState.Read.GetCell(protocol, Convert.ToString((int)requestType));
			if (original.Value == pollState)
			{
				return;
			}

			PollState.Read.SetCell(protocol, Convert.ToString((int)requestType), pollState);
			StateChanged?.Invoke(this, new PollManagerStateChangedEventArgs(protocol, requestType, pollState));
		}

		protected override void DisposeEvents()
		{
		}
	}
}
