using System;
using System.Collections.Generic;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol.PollManager;

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
			Dictionary<RequestType, PollManager.PollSettings> dic = PollManager.InitPollDictionary(protocol);
			PollManager.PollDeviceObjects(protocol, dic, DateTime.UtcNow);
		}
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}
