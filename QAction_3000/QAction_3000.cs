using System;
using System.Linq;

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
				var toBeRemoved = SLTables.Repositories.GetData(
					protocol,
					SLTables.Repositories.FullName.Read.Map<RepositoriesModel>(m => m.FullName),
					SLTables.Repositories.Owner.Read.Map<RepositoriesModel>(m => m.Owner),
					SLTables.Repositories.AutoRemove.Read.Map<RepositoriesModel>(m => m.AutoRemove))
						.Where(m => m.AutoRemove.HasValue && m.AutoRemove.Value)
						.Where(m => m.Owner == organization)
						.Select(m => m.FullName)
						.ToHashSet();

				SLTables.Repositories.DeleteRows(protocol, toBeRemoved);
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
