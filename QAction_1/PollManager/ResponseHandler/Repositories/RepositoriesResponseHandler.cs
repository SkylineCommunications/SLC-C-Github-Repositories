namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
    using System;
    using System.Linq;

    using Newtonsoft.Json;

    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
    using Skyline.Protocol.Extensions;
    using Skyline.Protocol.Tables;

    public static partial class RepositoriesResponseHandler
    {
        public static void HandleRepositoriesResponse(SLProtocol protocol)
        {
            // Check status code
            if (!protocol.IsSuccessStatusCode())
            {
                return;
            }

            // Parse response
            var response = JsonConvert.DeserializeObject<RepositoryResponse>(Convert.ToString(protocol.GetParameter(Parameter.getrepositorycontent)));
            var repo = new RepositoriesTableRow
            {
                Name = response.Name,
                FullName = response.FullName,
                Private = response.Private,
                Owner = response.Owner.Login,
                Description = response.Description,
                Fork = response.Fork,
                CreatedAt = response.CreatedAt,
                UpdatedAt = response.UpdatedAt,
                PushedAt = response.PushedAt,
                Size = response.Size,
                Stars = response.StargazersCount,
                Watcher = response.WatchersCount,
                Language = response.Language,
            };

            // Check if row exists, add or edit.
            var table = new RepositoriesTable(protocol);
            var exist = table.Rows.SingleOrDefault(row => row.FullName == response.FullName);
            if (exist == default(RepositoriesTableRow))
            {
                table.Rows.Add(repo);
            }
            else
            {
                var index = table.Rows.IndexOf(exist);
                table.Rows[index] = repo;
            }

            table.SaveToProtocol(protocol);
        }
    }
}
