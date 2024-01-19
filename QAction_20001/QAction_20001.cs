using System;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol.PollManager;
using Skyline.Protocol.PollManager.ResponseHandler;

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
			if (ResponseHandler.Handlers.TryGetValue((RequestType)trigger, out var handler))
			{
				handler(protocol);
			}
			else
			{
				protocol.Log($"QA{protocol.QActionID}|Run|No handler found for trigger parameter '{trigger}'", LogType.Error, LogLevel.NoLogging);
			}
		}
		catch (Exception ex)
		{
			protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
		}
	}
}
