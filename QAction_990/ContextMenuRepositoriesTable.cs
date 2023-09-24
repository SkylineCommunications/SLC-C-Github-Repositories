namespace QAction_990
{
    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.Table.ContextMenu;
    using Skyline.Protocol.Tables;

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
        }
    }
}