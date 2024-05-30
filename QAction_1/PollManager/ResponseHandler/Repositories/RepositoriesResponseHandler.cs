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
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.Protocol.API.Content;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.InterApp;
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
			row.Id = response.Id;

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

			// Check if there are Workflow InterApp messages waiting on content creation
			foreach (var iacRow in table.Rows.Where(iac => iac.ResponseType.AssemblyQualifiedName == typeof(AddWorkflowResponse).AssemblyQualifiedName))
			{
				var request = WorkflowMapping.FromMessage(iacRow.Request);
				var fileName = Path.GetFileNameWithoutExtension(response.Content.Name);
				var iacFileName = HttpUtility.UrlEncode(iacRow.Info);

				if (request.RepositoryId.Owner == owner &&
					request.RepositoryId.Name == name &&
					fileName == iacFileName)
				{
					var returnMessage = (GenericInterAppMessage<AddWorkflowResponse>)iacRow.Response;
					returnMessage.Data.Success = true;
					returnMessage.Data.Description = $"Successfully created new workflow called '{iacRow.Info}'.";
					iacRow.Request.Reply(protocol.SLNet.RawConnection, returnMessage, Types.KnownTypes);
					iacRow.Status = IAC_MessageStatus.Confirmed;
					iacRow.SaveToProtocol(protocol);

					RepositoriesRequestHandler.HandleRepositoriesWorkflowsRequest(protocol, request.RepositoryId.Owner, request.RepositoryId.Name, PollingConstants.PerPage, 1);
				}
			}

			// Check if there are generic InterApp messages waiting on content creation
			foreach (var iacRow in table.Rows.Where(iac => iac.ResponseType.AssemblyQualifiedName == typeof(CreateRepositoryContentResponse).AssemblyQualifiedName))
			{
				var request = (GenericInterAppMessage<CreateRepositoryContentRequest>)iacRow.Request;
				var filePath = response.Content.Name;
				var iacFilePath = iacRow.Info;

				if (request.Data.RepositoryId.Owner == owner &&
					request.Data.RepositoryId.Name == name &&
					filePath == iacFilePath)
				{
					var returnMessage = (GenericInterAppMessage<CreateRepositoryContentResponse>)iacRow.Response;
					returnMessage.Data.Success = true;
					returnMessage.Data.RepositoryPath = request.Data.RepositoryPath;
					returnMessage.Data.Description = $"Successfully created/updated '{iacRow.Info}'.";
					iacRow.Request.Reply(protocol.SLNet.RawConnection, returnMessage, Types.KnownTypes);
					iacRow.Status = IAC_MessageStatus.Confirmed;
					iacRow.SaveToProtocol(protocol);
				}
			}
		}
	}
}
