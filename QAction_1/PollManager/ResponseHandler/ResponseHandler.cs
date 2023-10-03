namespace Skyline.Protocol.PollManager.ResponseHandler
{
    using System;
    using System.Collections.Generic;

    using Skyline.DataMiner.Scripting;
    using Skyline.Protocol.PollManager.ResponseHandler.Repositories;

    public static class ResponseHandler
    {
        private static IReadOnlyDictionary<RequestType, Action<SLProtocol>> handlers = new Dictionary<RequestType, Action<SLProtocol>>
        {
            { RequestType.Repositories_Repositories,        RepositoriesResponseHandler.HandleRepositoriesResponse },
            { RequestType.Repositories_Tags,                RepositoriesResponseHandler.HandleRepositoriesTagsResponse },
            { RequestType.Repositories_Releases,            RepositoriesResponseHandler.HandleRepositoriesReleasesResponse },
            { RequestType.Repository_Issues,                RepositoriesResponseHandler.HandleRepositoriesIssuesResponse },
        };

        public static IReadOnlyDictionary<RequestType, Action<SLProtocol>> Handlers => handlers;
    }
}
