using System;
using System.Collections.Generic;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol.PollManager;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    private static readonly Dictionary<int, Action<SLProtocol>> QactionTriggers = new Dictionary<int, Action<SLProtocol>>
    {
        { Parameter.triggerrequesthandling_30, TriggerPolling },
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

    private static void TriggerPolling(SLProtocol protocol)
    {
        Dictionary<RequestType, PollManager.PollSettings> dic = PollManager.InitPollDictionary(protocol);
        PollManager.PollDeviceObjects(protocol, dic, DateTime.UtcNow);
    }
}
