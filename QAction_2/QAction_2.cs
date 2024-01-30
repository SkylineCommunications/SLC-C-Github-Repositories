using System;
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
            RepositoryTagsTable.GetTable(protocol);
            RepositoryReleasesTable.GetTable(protocol);
            RepositoryIssuesTable.GetTable(protocol);
            RepositoryWorkflowsTable.GetTable(protocol);
		}
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}
