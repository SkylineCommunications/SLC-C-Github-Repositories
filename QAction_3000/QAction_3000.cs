using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol.PollManager;
using Skyline.Protocol.Tables;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocol protocol)
	{
		try
		{
			var trigger = protocol.GetTriggerParameter();
			var organization = protocol.RowKey();
			var tracked = Convert.ToBoolean(protocol.GetParameter(trigger));

			// Enabling tracking
			if (tracked)
			{
				PollManager.ManualRefreshDeviceObject(protocol, RequestType.Organizations_Repositories, DateTime.Now);
			}

			// Disable tracking
			else
			{
				var reposTable = RepositoriesTable.GetTable(protocol);
				var toRemove = reposTable.Rows.Where(row => row.Owner == organization);
				reposTable.DeleteRow(protocol, toRemove.Select(row => row.FullName).ToArray());
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
