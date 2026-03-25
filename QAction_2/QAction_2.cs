using System;
using System.Runtime.CompilerServices;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol.PollManager;
using Skyline.Protocol.Tables;

/// <summary>
/// DataMiner QAction Class: After Startup.
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
			PollManager.InitPollManagerTableSettings(protocol, false);

			// Setup Table events
			RuntimeHelpers.RunClassConstructor(typeof(SLTables).TypeHandle);
			IAC_MessagesTable.GetTable(protocol);
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
