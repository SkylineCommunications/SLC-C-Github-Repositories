using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol;
using Skyline.Protocol.PollManager.RequestHandler.Organizations;
using Skyline.Protocol.PollManager.ResponseHandler;
using Skyline.Protocol.Tables;

/// <summary>
/// DataMiner QAction Class.
/// </summary>
public static class QAction
{
    private static Dictionary<int, Action<SLProtocol>> Handlers = new Dictionary<int, Action<SLProtocol>>
    {
        { Parameter.Write.addrepositorybutton_500, HandleIndividualAdd },
        { Parameter.Write.addorganizationrepositoriesbutton_505, HandleOrganizationAdd },
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
        var row = new RepositoriesTableRow
        {
            FullName = $"{parameters[1]}/{parameters[0]}",
            Name = Convert.ToString(parameters[0]),
            Owner = Convert.ToString(parameters[1]),
        };

        row.SaveToProtocol(protocol);

        protocol.SetParameters(ids, new object[] { Exceptions.NotAvailable, Exceptions.NotAvailable });
    }

    private static void HandleOrganizationAdd(SLProtocol protocol)
    {
        var org = Convert.ToString(protocol.GetParameter(Parameter.addorganizationrepositories_506));
        protocol.SetParameterIndexByKey(Parameter.Organizations.tablePid, org, Parameter.Organizations.Idx.organizationstrackrepositories + 1, Convert.ToDouble(true));
        OrganizationsRequestHandler.HandleOrganizationRepositoriesRequest(protocol, org);

        protocol.SetParameter(Parameter.repositories_changerequest, JsonConvert.SerializeObject(new AddRepositoriesTableRequest(org)));
    }
}
