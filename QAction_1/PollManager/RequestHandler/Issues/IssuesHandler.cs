namespace QAction_1.PollManager.RequestHandler.Issues
{
    using Skyline.DataMiner.Scripting;
    using Skyline.Protocol.Tables;

    public static class IssuesHandler
    {
        public static void HandleIssuesRepositoryRequest(SLProtocol protocol)
        {
            var table = new RepositoriesTable(protocol);
            foreach (var row in table.Rows)
            {
                protocol.SetParameter(Parameter.getrepositoryurl, $"repos/{row.Owner}/{row.Name}/issues");
                protocol.CheckTrigger(202);
            }
        }
    }
}
