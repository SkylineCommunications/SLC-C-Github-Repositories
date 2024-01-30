using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol.Extensions;
using Skyline.Protocol.PollManager.RequestHandler.Organizations;
using Skyline.Protocol.Tables;

using SLNetMessages = Skyline.DataMiner.Net.Messages;

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
            var param = Convert.ToString(protocol.GetParameter(Parameter.repositories_changerequest));
            var request = JsonConvert.DeserializeObject<RepositoryTableRequest>(param);
            switch (request.Action)
            {
                case RepositoryTableAction.Add:
                    Add(protocol, request);
                    break;

                case RepositoryTableAction.Remove:
                    Delete(protocol, request);
                    break;

                default:
                    throw new NotSupportedException("The give action is not supported yet.");
            }
        }
        catch (Exception ex)
        {
            protocol.Log($"QA{protocol.QActionID}|{protocol.GetTriggerParameter()}|Run|Exception thrown:{Environment.NewLine}{ex}", LogType.Error, LogLevel.NoLogging);
        }
    }

    private static void Add(SLProtocol protocol, IRepositoriesTableRequest request)
    {
        if(request.Organization == IndividualOrOrganization.Individual)
        {
            var row = new RepositoriesTableRow
            {
                FullName = $"{request.Data[0]}/{request.Data[1]}",
                Owner = request.Data[0],
                Name = request.Data[1],
            };

            row.SaveToProtocol(protocol);
        }
        else
        {
            var org = request.Data[0];
            protocol.SetParameterIndexByKey(Parameter.Organizations.tablePid, org, Parameter.Organizations.Idx.organizationstrackrepositories + 1, Convert.ToDouble(true));
            OrganizationsRequestHandler.HandleOrganizationRepositoriesRequest(protocol, org);
        }
    }

    private static void Delete(SLProtocol protocol, IRepositoriesTableRequest request)
    {
        var reposToDelete = request.Data;
        if (request.Organization == IndividualOrOrganization.Organization)
        {
            uint[] repositoriesTableIdx = new uint[]
            {
                Parameter.Repositories.Idx.repositoriesfullname,
                Parameter.Repositories.Idx.repositoriesowner,
            };
            object[] repositoriestable = (object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositories.tablePid, repositoriesTableIdx);
            object[] fullName = (object[])repositoriestable[0];
            object[] owner = (object[])repositoriestable[1];

            var orgRepos = new List<string>();
            for(int i = 0; i < owner.Length; i++)
            {
                if (Convert.ToString(owner[i]) != request.Data[0])
                    continue;

                orgRepos.Add(Convert.ToString(fullName[i]));
            }

            reposToDelete = orgRepos.ToArray();
        }

        RepositoriesTable.GetTable().DeleteRow(protocol, reposToDelete);
    }
}

internal class RepositoryTableRequest : IRepositoriesTableRequest
{
    public RepositoryTableAction Action { get; set; }

    public IndividualOrOrganization Organization { get; set; }

    public string[] Data { get; set; }
}
