using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Newtonsoft.Json;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol.PollManager.RequestHandler.Organizations;
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
            if(tracked)
            {
                protocol.SetParameter(Parameter.repositories_changerequest, JsonConvert.SerializeObject(new AddRepositoriesTableRequest(organization)));
            }

            // Disable tracking
            else
            {
                protocol.SetParameter(Parameter.repositories_changerequest, JsonConvert.SerializeObject(new RemoveRepositoriesTableRequest(IndividualOrOrganization.Organization, organization)));
            }
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }
}
