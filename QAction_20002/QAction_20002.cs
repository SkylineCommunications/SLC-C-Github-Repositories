using System;
using System.Collections.Generic;
using System.Linq;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol;
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
		{ Parameter.Pollmanager.Pid.Write.pollmanagerrefresh_21006,		RefreshPollItem },
		{ Parameter.Write.pollmanagerpollstate_21053,					StateChange },
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

		PollManager.ManualRefreshDeviceObject(protocol, requestType, DateTime.UtcNow);
	}

	private static void StateChange(SLProtocol protocol)
	{
		var requestType = (RequestType)Convert.ToInt32(protocol.RowKey());
		var state = (PollState)Convert.ToInt16(protocol.GetParameter(Parameter.Write.pollmanagerpollstate_21053));
		if(state == PollState.Enabled)
		{
			// Initiate a poll
			PollManager.ManualRefreshDeviceObject(protocol, requestType, DateTime.Now);
			return;
		}

		// If table is no longer polled clear it. This is to not have outdated information and to not have hanging alarms
		var tableId = requestType.GetTableID();
		if(tableId == -1)
		{
			// No table id defined in the QAction1/PollManager/RequestType.cs
			return;
		}

		// The repositories table is a special one, since there you can specify things manually and it get populated by multiple things
		// We can't just clear it when the polling is disabled
		if (tableId == Parameter.Repositories.tablePid)
		{
			var table = RepositoriesTable.GetTable();
			var pollTable = PollManagerTable.GetTable(protocol);

			if (requestType == RequestType.Repositories_PublicKey)
			{
				// Remove all the public keys
				foreach(var row in table.Rows)
				{
					row.PublicKeyID = Exceptions.NotAvailable;
					row.PublicKey = Exceptions.NotAvailable;
				}
			}
			else if(requestType == RequestType.Repositories_Repositories)
			{
				// If the Organization/Repositories is disabled then the orgs list should be empty.
				var orgRepoPolled = pollTable.Rows.Single(pollRow => pollRow.RequestType == RequestType.Organizations_Repositories).PollState == PollState.Enabled;
				var orgs = OrganizationsTable.GetTable().Rows.Where(x => x.Tracked && orgRepoPolled).Select(x => x.Instance);
				foreach(var row in table.Rows.Where(x => !orgs.Contains(x.Owner)))
				{
					row.SetToNotAvailable();
				}
			}
			else if(requestType == RequestType.Organizations_Repositories)
			{
				var orgs = OrganizationsTable.GetTable().Rows.Where(x => x.Tracked).Select(x => x.Instance);
				foreach(var row in table.Rows.Where(x => orgs.Contains(x.Owner)))
				{
					row.SetToNotAvailable();
				}
			}
			else
			{
				// Nothing to do here.
			}

			table.SaveToProtocol(protocol);
		}
		else
		{
			protocol.ClearAllKeys(tableId);
		}
	}
}
