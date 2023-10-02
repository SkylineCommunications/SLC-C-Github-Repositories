namespace QAction_990
{
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.Table.ContextMenu;
    using Skyline.Protocol.Extensions;
    using Skyline.Protocol.Tables;
    using System;
    using System.Linq;
    using SLNetMessages = Skyline.DataMiner.Net.Messages;

    internal enum Action
    {
        Add = 1,
        Deleteselectedrow_40_s_41_ = 2,
    }

    internal class ContextMenuRepositoriesTable : ContextMenu<Action>
    {
        public ContextMenuRepositoriesTable(SLProtocol protocol, object contextMenuData, int tablePid)
            : base(protocol, contextMenuData, tablePid)
        {
        }

        public override void ProcessContextMenuAction()
        {
            switch (Action)
            {
                case Action.Add:
                    Protocol.Log("QA" + Protocol.QActionID + "|ContextMenuRepositoriesTable|ProcessContextMenuAction|Add", LogType.DebugInfo, LogLevel.NoLogging);
                    Add();
                    break;

                case Action.Deleteselectedrow_40_s_41_:
                    Protocol.Log("QA" + Protocol.QActionID + "|ContextMenuRepositoriesTable|ProcessContextMenuAction|Deleteselectedrow_40_s_41_", LogType.DebugInfo, LogLevel.NoLogging);
                    Delete();
                    break;

                default:
                    Protocol.Log("QA" + Protocol.QActionID + "|ContextMenuRepositoriesTable|Process|Unexpected ContextMenu value '" + ActionRaw + "'", LogType.Error, LogLevel.NoLogging);
                    break;
            }
        }

        private void Add()
        {
            var row = new RepositoriesTableRow
            {
                FullName = $"{Data[1]}/{Data[0]}",
                Name = Data[0],
                Owner = Data[1],
            };

            row.SaveToProtocol(Protocol);
        }

        private void Delete()
        {
            Protocol.DeleteRow(Parameter.Repositories.tablePid, Data);
            Protocol.Log($"QA{Protocol.QActionID}|Delete|Repositories|Deleting '{Data.Count()}' rows", LogType.DebugInfo, LogLevel.NoLogging);

            foreach (var rowId in Data)
            {
                // Delete Linked Tags
                var tagsIdx = new uint[]
                {
                Parameter.Repositorytags.Idx.repositorytagsid,
                Parameter.Repositorytags.Idx.repositorytagsrepositoryid,
                };
                var tagRows = ((object[])Protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositorytags.tablePid, tagsIdx))
                    .Select(col => Array.ConvertAll((object[])col, Convert.ToString))
                    .ToRows()
                    .Where(row => row[1] == rowId)
                    .Select(row => row[0]);
                Protocol.Log($"QA{Protocol.QActionID}|Delete|{rowId}|Tags|Deleting '{tagRows.Count()}' rows", LogType.DebugInfo, LogLevel.NoLogging);
                Protocol.DeleteRow(Parameter.Repositorytags.tablePid, tagRows.ToArray());

                // Delete Linked Releases
                var releasesIdx = new uint[]
                {
                Parameter.Repositoryreleases.Idx.repositoryreleasesinstance,
                Parameter.Repositoryreleases.Idx.repositoryreleasesrepositoryid,
                };
                var releaseRows = ((object[])Protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryreleases.tablePid, releasesIdx))
                    .Select(col => Array.ConvertAll((object[])col, Convert.ToString))
                    .ToRows()
                    .Where(row => row[1] == rowId)
                    .Select(row => row[0]);
                Protocol.Log($"QA{Protocol.QActionID}|Delete|{rowId}|Releases|Deleting '{releaseRows.Count()}' rows", LogType.DebugInfo, LogLevel.NoLogging);
                Protocol.DeleteRow(Parameter.Repositoryreleases.tablePid, releaseRows.ToArray());

                // Delete Linked Issues
                var issuesIdx = new uint[]
                {
                Parameter.Repositoryissues.Idx.repositoryissuesinstance,
                Parameter.Repositoryissues.Idx.repositoryissuesrepositoryid,
                };
                var issuesRows = ((object[])Protocol.NotifyProtocol((int)SLNetMessages.NotifyType.NT_GET_TABLE_COLUMNS, Parameter.Repositoryissues.tablePid, issuesIdx))
                    .Select(col => Array.ConvertAll((object[])col, Convert.ToString))
                    .ToRows()
                    .Where(row => row[1] == rowId)
                    .Select(row => row[0]);
                Protocol.Log($"QA{Protocol.QActionID}|Delete|{rowId}|Issues|Deleting '{issuesRows.Count()}' rows", LogType.DebugInfo, LogLevel.NoLogging);
                Protocol.DeleteRow(Parameter.Repositoryissues.tablePid, issuesRows.ToArray());
            }
        }
    }
}