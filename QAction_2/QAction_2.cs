using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using QAction_1.PollManager;
using Skyline.DataMiner.Scripting;

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
            PollManager.InitPollManagerTableSettings(protocol);
            protocol.ClearAllKeys(Parameter.Repositorytags.tablePid);
            protocol.ClearAllKeys(Parameter.Repositoryreleases.tablePid);
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}
