using System;
using System.Collections.Generic;

using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol;
using Skyline.Protocol.InterApp;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    private static Dictionary<int, Action<SLProtocol>> Handlers = new Dictionary<int, Action<SLProtocol>>
    {
        { Parameter.Write.addrepositorybutton_500, HandleIndividualAdd },
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
            if (Handlers.TryGetValue(trigger, out var handler))
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

    private static void HandleIndividualAdd(SLProtocol protocol)
    {
        var ids = new[]
        {
            Parameter.addrepositoryname,
            Parameter.addrepositoryowner,
        };
        var parameters = (object[])protocol.GetParameters(Array.ConvertAll(ids, Convert.ToUInt32));

        // Add through name and owner
        var request = new AddRepositoryRequest
        {
            RepositoryId = new RepositoryId(Convert.ToString(parameters[1]), Convert.ToString(parameters[0])),
        };

        request.TryExecute(protocol, protocol, Mapping.MessageToExecutorMapping, out _);

        protocol.SetParameters(ids, new object[] { Exceptions.NotAvailable, Exceptions.NotAvailable });
    }
}
