namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Linq;
	using System.Text.RegularExpressions;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesResponseHandler
	{
		public static void HandleRepositoriesAddRepositoryCollaboratorResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// If response success it was added correctly
			var url = Convert.ToString(protocol.GetParameter(Parameter.putrepositoryusercollaboratorurl));

			// Parse url to check which organization this member is linked to
			var pattern = "repos\\/(?<owner>.*)\\/(?<name>.*)\\/collaborators\\/(?<user>.*)";
			var options = RegexOptions.Multiline;

			var match = Regex.Match(url, pattern, options);
			var user = match.Groups["user"].Value;
			var owner = match.Groups["owner"].Value;
			var name = match.Groups["name"].Value;


			// Check if there are generic InterApp messages waiting on content creation
			var table = IAC_MessagesTable.GetTable(protocol);
			foreach (var iacRow in table.Rows.Where(iac => iac.ResponseType.AssemblyQualifiedName == typeof(AddRepositoryCollaboratorResponse).AssemblyQualifiedName))
			{
				var request = (GenericInterAppMessage<AddRepositoryCollaboratorRequest>)iacRow.Request;
				if (request.Data.Data.CollaboratorType != CollaboratorType.User)
				{
					continue;
				}

				var iacOwner = iacRow.Info.Split('/')[0];
				var iacName = iacRow.Info.Split('/')[1];
				var iacUser = iacRow.Info.Split('/')[2];

				if (user == iacUser &&
					owner == iacOwner &&
					name == iacName)
				{
					var returnMessage = (GenericInterAppMessage<AddRepositoryCollaboratorResponse>)iacRow.Response;
					returnMessage.Data.Success = true;
					returnMessage.Data.RepositoryId = new RepositoryId(owner, name);
					returnMessage.Data.Description = $"Successfully added user '{user}'.";
					iacRow.Request.Reply(protocol.SLNet.RawConnection, returnMessage, Types.KnownTypes);
					iacRow.Status = IAC_MessageStatus.Confirmed;
					iacRow.SaveToProtocol(protocol);
				}
			}
		}
	}
}
