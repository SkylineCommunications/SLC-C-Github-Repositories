namespace QAction_1.PollManager.RequestHandler
{
    using System;
    using System.Collections.Generic;
    using QAction_1.PollManager.RequestHandler.Issues;
    using QAction_1.PollManager.RequestHandler.Repositories;
    using Skyline.DataMiner.Scripting;

    public static class RequestHandler
    {
        public static IReadOnlyDictionary<RequestType, Action<SLProtocol>> Handlers = new Dictionary<RequestType, Action<SLProtocol>>
        {
            { RequestType.Repositories_Repositories,        RepositoriesHandler.HandleRepositoriesRequest },
            { RequestType.Repositories_Tags,                RepositoriesHandler.HandleRepositoriesTagsRequest },
            { RequestType.Repositories_Releases,            RepositoriesHandler.HandleRepositoriesReleasesRequest },
            { RequestType.Repository_Issues,                RepositoriesHandler.HandleIssuesRepositoryRequest },
        };
    }
}
