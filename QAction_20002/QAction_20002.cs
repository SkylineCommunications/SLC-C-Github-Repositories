using System;
using System.Collections.Generic;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol.Extensions;
using Skyline.Protocol.PollManager;
using Skyline.Protocol.Tables;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
	private static readonly Dictionary<int, Action<SLProtocol>> QactionTriggers = new Dictionary<int, Action<SLProtocol>>
	{
		{ Parameter.Pollmanager.Pid.Write.pollmanagerrefresh_21006,     RefreshPollItem },
		{ Parameter.Write.pollmanagerpollstate_21053,                   StateChange },
	};

	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocol protocol)
	{
		try
		{
			int triggerId = protocol.GetTriggerParameter();
			if (!QactionTriggers.TryGetValue(triggerId, out Action<SLProtocol> handler))
			{
				throw new NotSupportedException($"Trigger with ID '{triggerId}' not supported.");
			}

			handler.Invoke(protocol);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}

	private static void RefreshPollItem(SLProtocol protocol)
	{
		var requestType = (RequestType)Convert.ToInt32(protocol.RowKey());
		if (!Convert.ToBoolean(protocol.GetParameterIndexByKey(Parameter.Pollmanager.tablePid, protocol.RowKey(), Parameter.Pollmanager.Idx.pollmanagerpollstate_21003 + 1)))
		{
			protocol.ShowInformationMessage($"Poll item '{requestType.FriendlyDescription()}' is set to Disabled.");
			return;
		}

		protocol.Log($"QA{protocol.QActionID}|RefreshPollItem|Polling '{requestType.FriendlyDescription()}'.", LogType.Information, LogLevel.Level2);
		PollManager.ManualRefreshDeviceObject(protocol, requestType, DateTime.UtcNow);
	}

	private static void StateChange(SLProtocol protocol)
	{
		var requestType = (RequestType)Convert.ToInt32(protocol.RowKey());
		var state = (PollState)Convert.ToInt16(protocol.GetParameter(Parameter.Write.pollmanagerpollstate_21053));
		SLTables.PollManager.SetPollState(protocol, requestType, state);
		if (state == PollState.Enabled)
		{
			// Initiate a poll
			PollManager.ManualRefreshDeviceObject(protocol, requestType, DateTime.Now);
		}
	}
}
