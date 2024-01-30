namespace QAction_990
{
    using Newtonsoft.Json;

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
                    Protocol.SetParameter(Parameter.repositories_changerequest, JsonConvert.SerializeObject(new AddRepositoriesTableRequest(Data[1], Data[0])));
                    break;

                case Action.Deleteselectedrow_40_s_41_:
                    Protocol.SetParameter(Parameter.repositories_changerequest, JsonConvert.SerializeObject(new RemoveRepositoriesTableRequest(IndividualOrOrganization.Individual, Data)));
                    break;

                default:
                    Protocol.Log("QA" + Protocol.QActionID + "|ContextMenuRepositoriesTable|Process|Unexpected ContextMenu value '" + ActionRaw + "'", LogType.Error, LogLevel.NoLogging);
                    break;
            }
        }
    }
}