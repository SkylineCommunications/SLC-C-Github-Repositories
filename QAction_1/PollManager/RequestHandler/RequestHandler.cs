namespace Skyline.Protocol.PollManager.RequestHandler
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.Scripting;
    using Skyline.Protocol.PollManager.RequestHandler.Repositories;

    public static class RequestHandler
    {
        private static IReadOnlyDictionary<RequestType, Action<SLProtocol>> handlers = new Dictionary<RequestType, Action<SLProtocol>>
        {
            { RequestType.Repositories_Repositories,        RepositoriesRequestHandler.HandleRepositoriesRequest },
            { RequestType.Repositories_Tags,                RepositoriesRequestHandler.HandleRepositoriesTagsRequest },
            { RequestType.Repositories_Releases,            RepositoriesRequestHandler.HandleRepositoriesReleasesRequest },
            { RequestType.Repository_Issues,                RepositoriesRequestHandler.HandleRepositoriesIssuesRequest },
        };

        public static IReadOnlyDictionary<RequestType, Action<SLProtocol>> Handlers => handlers;
    }
}
