namespace Skyline.Protocol.PollManager.ResponseHandler.Organizations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Scripting.Helper;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Repositories;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
	using Skyline.Protocol.API.Headers;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager.RequestHandler.Organizations;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.PollManager.ResponseHandler.Repositories;
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
			var response = SecureNewtonsoftDeserialization.DeserializeObject<List<RepositoryResponse>>(
				Convert.ToString(protocol.GetParameter(Parameter.getorganizationrepositoriescontent)));
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

			var rows = new List<RepositoriesQActionRow>();
			var existingRows = SLTables.Repositories.GetData(protocol,
				SLTables.Repositories.FullName.Read.Map<RepositoriesModel>(m => m.FullName),
				SLTables.Repositories.PublicKey.Read.Map<RepositoriesModel>(m => m.PublicKey),
				SLTables.Repositories.PublicKeyID.Read.Map<RepositoriesModel>(m => m.PublicKeyID))
					.ToDictionary(m => m.FullName);
			foreach (var repo in response)
			{
				// Update existing organization if found, otherwise create new one
				if (!existingRows.TryGetValue(repo.FullName, out var row))
				{
					row = new RepositoriesModel
					{
						AutoRemove = true,
					};
				}

				row.FullName = repo.FullName;
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
				row.Type = RepositoriesModel.GetTypeFromTopics(repo.Topics);
				row.Id = repo.Id;

				rows.Add(RepositoriesRowConverter.Instance.ToRawValue(row));
			}

			if (rows.Count > 0)
			{
				SLTables.Repositories.FillTableNoDelete(protocol, rows);
			}

			// Check if there are more repositories to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getorganizationrepositorieslinkheader));
			var link = new LinkHeader(linkHeader);

			if (link.HasNext)
			{
				OrganizationsRequestHandler.HandleOrganizationRepositoriesRequest(protocol, response[0].Owner.Login, link.NextPage, true);
			}
		}

		public static void HandleOrganizationCreateRepositoryResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = SecureNewtonsoftDeserialization.DeserializeObject<RepositoryResponse>(
				Convert.ToString(protocol.GetParameter(Parameter.postrepositorycontent)));
			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|HandleOrganizationCreateRepositoryResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			// Update existing organization if found, otherwise create new one
			var row = new RepositoriesModel();
			if (SLTables.Repositories.TryGetRow(protocol, response.FullName, out var rawRow))
			{
				row = RepositoriesRowConverter.Instance.FromRawValue(rawRow);
			}

			row.FullName = response.FullName;
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
			row.Type = RepositoriesModel.GetTypeFromTopics(response.Topics);
			row.Id = response.Id;
			row.AutoRemove = false;
			row.Topics.Clear();
			row.Topics.AddRange(response.Topics);

			SLTables.Repositories.SetRow(protocol, RepositoriesRowConverter.Instance.ToRawValue(row));

			RepositoriesRequestHandler.HandleRepositoriesPublicKeysRequest(protocol, response.FullName, true);

			HandleInterAppResponses(protocol, response);
			RepositoriesResponseHandler.HandleTopicsInterApp(protocol, response.Name, response.Owner.Login, response.Topics);
		}

		private static void HandleInterAppResponses(SLProtocol protocol, RepositoryResponse response)
		{
			// Check if there are generic InterApp messages waiting on content creation
			var table = IAC_MessagesTable.GetTable(protocol);
			foreach (var iacRow in table.Rows.Where(iac => iac.ResponseType.AssemblyQualifiedName == typeof(CreateRepositoryResponse).AssemblyQualifiedName))
			{
				var owner = response.Owner.Login;
				var name = response.Name;

				var iacOwner = iacRow.Info.Split('/')[0];
				var iacName = iacRow.Info.Split('/')[1];

				if (owner == iacOwner &&
					name == iacName)
				{
					var returnMessage = (GenericInterAppMessage<CreateRepositoryResponse>)iacRow.Response;
					returnMessage.Data.Success = true;
					returnMessage.Data.RepositoryId = new RepositoryId(owner, name);
					returnMessage.Data.Description = $"Successfully created '{iacRow.Info}'.";
					iacRow.Request.Reply(protocol.SLNet.RawConnection, returnMessage, Types.KnownTypes);
					iacRow.Status = IAC_MessageStatus.Confirmed;
					iacRow.SaveToProtocol(protocol);
				}
			}
		}
	}
}
