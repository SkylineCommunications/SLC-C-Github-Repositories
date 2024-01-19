namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
    using Skyline.DataMiner.Scripting;
    using Skyline.Protocol.Tables;

    public static partial class RepositoriesRequestHandler
    {
        public static void HandleRepositoriesRequest(SLProtocol protocol)
        {
            var table = RepositoriesTable.GetTable(protocol);
            foreach (var row in table.Rows)
            {
                protocol.SetParameter(Parameter.getrepositoryurl, $"repos/{row.Owner}/{row.Name}");
                protocol.CheckTrigger(201);
            }
        }
    }
}
