using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Protocol.Extension;

/// <summary>
/// DataMiner QAction Class: IAC Messages Cleanup.
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
			var interval = TimeSpan.FromSeconds(Convert.ToDouble(protocol.GetParameter(Parameter.maxtimeforiacmessages_9000049)));
			var timeToDelete = DateTime.Now - interval;

			var columns = protocol.GetColumns(Parameter.Iac_messages.tablePid, new uint[] { Parameter.Iac_messages.Idx.iac_messagesguid_9000101, Parameter.Iac_messages.Idx.iac_messagesrequesttime_9000108 });
			var keys = (object[])columns[0];
			var times = (object[])columns[1];

			var keysToDelete = new List<string>();
			for (int i = 0; i < keys.Length; i++)
			{
				if (DateTime.FromOADate(Convert.ToDouble(times[i])) < timeToDelete)
				{
					keysToDelete.Add(Convert.ToString(keys[i]));
				}
			}

			protocol.DeleteRows(Parameter.Iac_messages.tablePid, keysToDelete);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}