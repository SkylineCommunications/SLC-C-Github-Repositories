namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Web;

	using Newtonsoft.Json;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.Protocol.API.Content;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
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

		public static void HandleRepositoryContentResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = JsonConvert.DeserializeObject<PutRepositoryContentResponse>(Convert.ToString(protocol.GetParameter(Parameter.putrepositorycontentcontent)));
			var url = response.Content.Url;
			var table = IAC_MessagesTable.GetTable(protocol);

			// Parse url to check which respository this issue is linked to
			var pattern = "repos\\/(.*)\\/(.*)\\/contents\\/(.*)";
			var options = RegexOptions.Multiline;

			var match = Regex.Match(url, pattern, options);
			var owner = match.Groups[1].Value;
			var name = match.Groups[2].Value;

			// Check if there are InterApp messages waiting on content creation
			foreach (var iacRow in table.Rows.Where(iac => iac.ResponseType.AssemblyQualifiedName == typeof(AddWorkflowResponse).AssemblyQualifiedName))
			{
				var request = (AddWorkflowRequest)iacRow.Request;
				var fileName = Path.GetFileNameWithoutExtension(response.Content.Name);
				var iacFileName = HttpUtility.UrlEncode(iacRow.Info);

				if (request.RepositoryId.Owner == owner &&
					request.RepositoryId.Name == name &&
					fileName == iacFileName)
				{
					var returnMessage = (AddWorkflowResponse)iacRow.Response;
					returnMessage.Success = true;
					returnMessage.Description = $"Successfully created new workflow called '{iacRow.Info}'.";
					iacRow.Request.Reply(protocol.SLNet.RawConnection, returnMessage, Types.KnownTypes);
					iacRow.Status = IAC_MessageStatus.Confirmed;
					iacRow.SaveToProtocol(protocol);

					RepositoriesRequestHandler.HandleRepositoriesWorkflowsRequest(protocol, request.RepositoryId.Owner, request.RepositoryId.Name, PollingConstants.PerPage, 1);
				}
			}
		}
	}
}
