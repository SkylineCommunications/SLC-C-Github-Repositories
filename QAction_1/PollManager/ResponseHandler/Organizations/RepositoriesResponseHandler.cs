namespace Skyline.Protocol.PollManager.ResponseHandler.Organizations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.Protocol.API.Headers;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager.RequestHandler.Organizations;
	using Skyline.Protocol.Tables;

	public static partial class OrganizationsResponseHandler
	{
		public static void HandleOrganizationRepositoriesResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = JsonConvert.DeserializeObject<List<RepositoryResponse>>(Convert.ToString(protocol.GetParameter(Parameter.getorganizationrepositoriescontent)));
			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|ParseGetOrganizationRepositoriesResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			if (!response.Any())
			{
				// No repositories for the organization
				protocol.Log($"QA{protocol.QActionID}|ParseGetOrganizationRepositoriesResponse|No repositories", LogType.Information, LogLevel.Level2);
				return;
			}

			var table = RepositoriesTable.GetTable();
			foreach (var repo in response)
			{
				// Update existing organization if found, otherwise create new one
				var row = table.Rows.Find(repository => repository.FullName == repo.FullName) ?? new RepositoriesTableRow();
				row.Name = repo.Name;
				row.Private = repo.Private;
				row.Owner = repo.Owner.Login;
				row.Description = repo.Description;
				row.Fork = repo.Fork;
				row.CreatedAt = repo.CreatedAt;
				row.UpdatedAt = repo.UpdatedAt;
				row.PushedAt = repo.PushedAt;
				row.Size = repo.Size;
				row.Stars = repo.StargazersCount;
				row.Watcher = repo.WatchersCount;
				row.Language = repo.Language;
				row.DefaultBranch = repo.DefaultBranch;
				row.Type = RepositoriesTableRow.GetTypeFromTopics(repo.Topics);

				// If its a new row fill in ID and add it to the table.
				if (row.FullName == Exceptions.NotAvailable)
				{
					row.FullName = repo.FullName;
					table.Rows.Add(row);
				}
			}

			if (table.Rows.Count > 0)
			{
				table.SaveToProtocol(protocol, true);
			}

			// Check if there are more repositories to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getorganizationrepositorieslinkheader));
			if (string.IsNullOrEmpty(linkHeader)) return;

			var link = new LinkHeader(linkHeader);

			if (link.HasNext)
			{
				OrganizationsRequestHandler.HandleOrganizationRepositoriesRequest(protocol, response[0].Owner.Login, PollingConstants.PerPage, link.NextPage);
			}
		}
	}
}
