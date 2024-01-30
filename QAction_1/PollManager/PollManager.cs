namespace Skyline.Protocol.PollManager
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Extensions;

	using Parameter = Skyline.DataMiner.Scripting.Parameter;

	public static class PollManager
	{
		private static readonly Dictionary<RequestType, PollSettings> InitialSettings = new Dictionary<RequestType, PollSettings>
		{
			{ RequestType.Repositories_Repositories, new PollSettings { PollFrequency = TimeSpan.FromMinutes(10), Enabled = true } },
			{ RequestType.Repositories_Tags, new PollSettings { PollFrequency = TimeSpan.FromMinutes(360), Enabled = true } },
			{ RequestType.Repositories_Releases, new PollSettings { PollFrequency = TimeSpan.FromMinutes(360), Enabled = true } },
			{ RequestType.Repository_Issues, new PollSettings { PollFrequency = TimeSpan.FromMinutes(5), Enabled = true } },
			{ RequestType.Repositories_Workflows, new PollSettings { PollFrequency = TimeSpan.FromMinutes(360), Enabled = true } },

			{ RequestType.Organizations_User, new PollSettings { PollFrequency = TimeSpan.FromHours(24), Enabled = true } },
			{ RequestType.Organizations_Repositories, new PollSettings { PollFrequency = TimeSpan.FromHours(10), Enabled = true } },

			{ RequestType.Repositories_PublicKey, new PollSettings { PollFrequency = TimeSpan.FromHours(360), Enabled = true } },
		};

		/// <summary>
		///     Returns the time difference between the last poll and the poll before that.
		/// </summary>
		/// <param name="protocol">SLProtocol reference.</param>
		/// <param name="requestType">Request Type you wish to know the difference for.</param>
		/// <returns>Timespan between the last and previous poll cycle.</returns>
		public static TimeSpan GetTimeDifferenceBetweenLastPolls(SLProtocol protocol, RequestType requestType)
		{
			var row = new PollmanagerQActionRow((object[])protocol.GetRow(Parameter.Pollmanager.tablePid, Convert.ToString((int)requestType)));
			double lastPoll = Convert.ToDouble(row.Pollmanagerlastpolled_21005);
			double previousPoll = Convert.ToDouble(row.Pollmanagerpreviouslypolled_21007);
			if (lastPoll < 1 || previousPoll < 1)
			{
				return new TimeSpan(0);
			}

			return DateTime.FromOADate(lastPoll) - DateTime.FromOADate(previousPoll);
		}

		public static Dictionary<RequestType, PollSettings> InitPollDictionary(SLProtocol protocol)
		{
			var pollManagerIdx = new uint[]
			{
				Parameter.Pollmanager.Idx.pollmanagerindex_21001,
				Parameter.Pollmanager.Idx.pollmanagerpollstate_21003,
				Parameter.Pollmanager.Idx.pollmanagerpollfrequency_21004,
				Parameter.Pollmanager.Idx.pollmanagerlastpolled_21005,
			};
			var pollManagerColumns = (object[])protocol.NotifyProtocol((int)NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Pollmanager.tablePid, pollManagerIdx);
			uint[] pollManagerKeys = Array.ConvertAll((object[])pollManagerColumns[0], Convert.ToUInt32);
			bool[] pollManagerStates = Array.ConvertAll((object[])pollManagerColumns[1], Convert.ToBoolean);
			TimeSpan[] pollManagerFrequencies = Array.ConvertAll((object[])pollManagerColumns[2], x => TimeSpan.FromSeconds(Convert.ToInt32(x)));
			DateTime[] pollManagerLastPolled = Array.ConvertAll((object[])pollManagerColumns[3], x => DateTime.FromOADate(Convert.ToDouble(x)));

			var dic = new Dictionary<RequestType, PollSettings>();
			for (var i = 0; i < pollManagerKeys.Length; i++)
			{
				dic.Add((RequestType)pollManagerKeys[i], new PollSettings { Enabled = pollManagerStates[i], PollFrequency = pollManagerFrequencies[i], LastPollTime = pollManagerLastPolled[i] });
			}

			return dic;
		}

		public static void InitPollManagerTableSettings(SLProtocol protocol, bool forced = false)
		{
			if (forced)
			{
				protocol.ClearAllKeys(Parameter.Pollmanager.tablePid);
			}

			if (protocol.RowCount(Parameter.Pollmanager.tablePid) == InitialSettings.Count)
			{
				object[] keys = InitialSettings.Select(x => Convert.ToString((int)x.Key)).ToArray<object>();
				object[] values = new int[keys.Length].OfType<object>().ToArray();
				protocol.FillArrayWithColumn(Parameter.Pollmanager.tablePid, Parameter.Pollmanager.Pid.pollmanagerlastpolled_21005, keys, values);
				return;
			}

			var pollManagerRows = new List<object[]>();
			foreach (KeyValuePair<RequestType, PollSettings> kvp in InitialSettings)
			{
				pollManagerRows.Add(
					new PollmanagerQActionRow
					{
						Pollmanagerindex_21001 = (int)kvp.Key,
						Pollmanagername_21002 = kvp.Key.FriendlyDescription(),
						Pollmanagerpollstate_21003 = Convert.ToInt32(kvp.Value.Enabled),
						Pollmanagerpollfrequency_21004 = kvp.Value.PollFrequency.TotalSeconds,
						Pollmanagerlastpolled_21005 = 0,
					}.ToObjectArray());
			}

			protocol.FillArray(Parameter.Pollmanager.tablePid, pollManagerRows, NotifyProtocol.SaveOption.Full);
		}

		public static void ManualRefreshDeviceObject(SLProtocol protocol, RequestType requestType, DateTime utcNow)
		{
			PollDeviceObject(protocol, requestType, utcNow);
		}

		public static void ManualRefreshDeviceObjects(SLProtocol protocol, List<RequestType> requestTypes, DateTime utcNow)
		{
			foreach (RequestType requestType in requestTypes)
			{
				PollDeviceObject(protocol, requestType, utcNow);
			}
		}

		public static void PollDeviceObjects(SLProtocol protocol, Dictionary<RequestType, PollSettings> pollItems, DateTime utcNow)
		{
			foreach (KeyValuePair<RequestType, PollSettings> pollItem in pollItems.OrderBy(x => (int)x.Key))
			{
				if (!pollItem.Value.Enabled
					|| pollItem.Value.LastPollTime + pollItem.Value.PollFrequency > utcNow)
				{
					continue;
				}

				PollDeviceObject(protocol, pollItem.Key, utcNow);
			}
		}

		private static void PollDeviceObject(SLProtocol protocol, RequestType requestType, DateTime utcNow)
		{
			if (RequestHandler.RequestHandler.Handlers.TryGetValue(requestType, out Action<SLProtocol> action))
			{
				UpdateLastPollTime(protocol, requestType, utcNow);
				action.Invoke(protocol);
			}
		}

		private static void UpdateLastPollTime(SLProtocol protocol, RequestType requestType, DateTime dt)
		{
			string rowKey = Convert.ToString((int)requestType);
			if (!protocol.Exists(Parameter.Pollmanager.tablePid, rowKey)) return;
			var row = new PollmanagerQActionRow((object[])protocol.GetRow(Parameter.Pollmanager.tablePid, rowKey));
			var lastPolled = row.Pollmanagerlastpolled_21005;

			row.Pollmanagerpreviouslypolled_21007 = lastPolled;
			row.Pollmanagerlastpolled_21005 = dt.ToOADate();

			protocol.SetRow(Parameter.Pollmanager.tablePid, rowKey, row.ToObjectArray());
		}

		public sealed class PollSettings
		{
			public bool Enabled { get; set; }

			public DateTime LastPollTime { get; set; }

			public TimeSpan PollFrequency { get; set; }
		}
	}
}