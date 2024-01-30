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

            var table = RepositoriesTable.GetTable();
            var row = table.Rows.Find(repository => repository.FullName == response.FullName) ?? new RepositoriesTableRow();
            row.Name = response.Name;
            row.Private = response.Private;
            row.Owner = response.Owner.Login;
            row.Description = response.Description;
            row.Fork = response.Fork;
            row.CreatedAt = response.CreatedAt;
            row.UpdatedAt = response.UpdatedAt;
            row.PushedAt = response.PushedAt;
            row.Size = response.Size;
            row.Stars = response.StargazersCount;
            row.Watcher = response.WatchersCount;
            row.Language = response.Language;
            row.DefaultBranch = response.DefaultBranch;
            row.Type = RepositoriesTableRow.GetTypeFromTopics(response.Topics);

            // If its a new row fill in ID and add it to the table.
            if (row.FullName == Exceptions.NotAvailable)
            {
                row.FullName = response.FullName;
                table.Rows.Add(row);
            }

            table.SaveToProtocol(protocol);
        }
    }
}
