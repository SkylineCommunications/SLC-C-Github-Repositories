using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Newtonsoft.Json;

using Skyline.DataMiner.Scripting;
using Skyline.Protocol.Extensions;
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
        var row = new RepositoriesTableRow
        {
            FullName = $"{request.Data[0]}/{request.Data[1]}",
            Owner = request.Data[0],
            Name = request.Data[1],
        };

        row.SaveToProtocol(protocol);
    }

    private static void Delete(SLProtocol protocol, IRepositoriesTableRequest request)
    {
        protocol.DeleteRow(Parameter.Repositories.tablePid, request.Data);
        protocol.Log($"QA{protocol.QActionID}|Delete|Repositories|Deleting '{request.Data.Count()}' rows", LogType.DebugInfo, LogLevel.NoLogging);

        foreach (var rowId in request.Data)
        {
            // Delete Linked Tags
            var tagsIdx = new uint[]
            {
                Parameter.Repositorytags.Idx.repositorytagsid,
                Parameter.Repositorytags.Idx.repositorytagsrepositoryid,
            };
            var tagRows = ((object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorytags.tablePid, tagsIdx))
                .Select(col => Array.ConvertAll((object[])col, Convert.ToString))
                .ToRows()
                .Where(row => row[1] == rowId)
                .Select(row => row[0]);
            protocol.Log($"QA{protocol.QActionID}|Delete|{rowId}|Tags|Deleting '{tagRows.Count()}' rows", LogType.DebugInfo, LogLevel.NoLogging);
            protocol.DeleteRow(Parameter.Repositorytags.tablePid, tagRows.ToArray());

            // Delete Linked Releases
            var releasesIdx = new uint[]
            {
                Parameter.Repositoryreleases.Idx.repositoryreleasesinstance,
                Parameter.Repositoryreleases.Idx.repositoryreleasesrepositoryid,
            };
            var releaseRows = ((object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryreleases.tablePid, releasesIdx))
                .Select(col => Array.ConvertAll((object[])col, Convert.ToString))
                .ToRows()
                .Where(row => row[1] == rowId)
                .Select(row => row[0]);
            protocol.Log($"QA{protocol.QActionID}|Delete|{rowId}|Releases|Deleting '{releaseRows.Count()}' rows", LogType.DebugInfo, LogLevel.NoLogging);
            protocol.DeleteRow(Parameter.Repositoryreleases.tablePid, releaseRows.ToArray());

            // Delete Linked Issues
            var issuesIdx = new uint[]
            {
                Parameter.Repositoryissues.Idx.repositoryissuesinstance,
                Parameter.Repositoryissues.Idx.repositoryissuesrepositoryid,
            };
            var issuesRows = ((object[])protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryissues.tablePid, issuesIdx))
                .Select(col => Array.ConvertAll((object[])col, Convert.ToString))
                .ToRows()
                .Where(row => row[1] == rowId)
                .Select(row => row[0]);
            protocol.Log($"QA{protocol.QActionID}|Delete|{rowId}|Issues|Deleting '{issuesRows.Count()}' rows", LogType.DebugInfo, LogLevel.NoLogging);
            protocol.DeleteRow(Parameter.Repositoryissues.tablePid, issuesRows.ToArray());
        }
    }
}

internal class RepositoryTableRequest : IRepositoriesTableRequest
{
    public RepositoryTableAction Action { get; set; }

    public string[] Data { get; set; }
}
