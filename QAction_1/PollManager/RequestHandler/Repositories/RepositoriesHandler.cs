namespace QAction_1.PollManager.RequestHandler.Repositories
{
    using Skyline.DataMiner.Scripting;
    using Skyline.Protocol.Tables;

    public static class RepositoriesHandler
    {
        public static void HandleRepositoriesRequest(SLProtocol protocol)
        {
            var table = new RepositoriesTable(protocol);
            foreach (var row in table.Rows)
            {
                protocol.SetParameter(Parameter.getrepositoryurl, $"repos/{row.Owner}/{row.Name}");
                protocol.CheckTrigger(201);
            }
        }

        public static void HandleIssuesRepositoryRequest(SLProtocol protocol)
        {
            var table = new RepositoriesTable(protocol);
            foreach (var row in table.Rows)
            {
                protocol.SetParameter(Parameter.getrepositoryissuesurl, $"repos/{row.Owner}/{row.Name}/issues?state=all");
                protocol.CheckTrigger(202);
            }
        }

        public static void HandleRepositoriesTagsRequest(SLProtocol protocol)
        {
            var table = new RepositoriesTable(protocol);
            foreach (var row in table.Rows)
            {
                protocol.SetParameter(Parameter.getrepositorytagsurl, $"repos/{row.Owner}/{row.Name}/tags");
                protocol.CheckTrigger(203);
            }
        }

        public static void HandleRepositoriesReleasesRequest(SLProtocol protocol)
        {
            var table = new RepositoriesTable(protocol);
            foreach (var row in table.Rows)
            {
                protocol.SetParameter(Parameter.getrepositoryreleasesurl, $"repos/{row.Owner}/{row.Name}/releases");
                protocol.CheckTrigger(204);
            }
        }
    }
}
