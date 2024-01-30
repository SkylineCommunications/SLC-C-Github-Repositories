using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Skyline.DataMiner.Scripting;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    private static IReadOnlyDictionary<int, Action<SLProtocol>> handlers = new Dictionary<int, Action<SLProtocol>>
    {
        { Parameter.Write.apikey,                   ApiKeyHandler },
        { Parameter.ratelimitresetepochseconds,     EpochHandler },
    };

    /// <summary>
    /// The QAction entry point.
    /// </summary>
    /// <param name="protocol">Link with SLProtocol process.</param>
    public static void Run(SLProtocol protocol)
    {
        try
        {
            var trigger = protocol.GetTriggerParameter();
            if(handlers.ContainsKey(trigger))
            {
                handlers[trigger](protocol);
            }
            else
            {
                protocol.Log($"QA{protocol.QActionID}|Run|Trigger '{trigger}' is not supported", LogType.Error, LogLevel.NoLogging);
            }
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }

    private static void ApiKeyHandler(SLProtocol protocol)
    {
        var trigger = protocol.GetTriggerParameter();
        var key = Convert.ToString(protocol.GetParameter(trigger));
        protocol.SetParameter(Parameter.apikeybearer, $"Bearer {key}");
    }

    private static void EpochHandler(SLProtocol protocol)
    {
        var trigger = protocol.GetTriggerParameter();
        var epochSeconds = Convert.ToInt32(protocol.GetParameter(trigger));

        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(epochSeconds);
        DateTime dateTime = dateTimeOffset.ToLocalTime().DateTime;

        protocol.SetParameter(trigger - 1, dateTime.ToOADate());
    }
}
