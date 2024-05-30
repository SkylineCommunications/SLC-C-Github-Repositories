namespace Skyline.Protocol.PollManager.ResponseHandler.Organizations
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

	public static partial class OrganizationsResponseHandler
	{
		public static void HandleOrganizationAddRepositoryCollaboratorResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// If response success it was added correctly
			var url = Convert.ToString(protocol.GetParameter(Parameter.putrepositoryteamcollaboratorurl));

			// Parse url to check which organization this member is linked to
			var pattern = "orgs\\/(?<organization>.*)\\/teams\\/(?<team>.*)\\/repos\\/(?<owner>.*)\\/(?<name>.*)";
			var options = RegexOptions.Multiline;

			var match = Regex.Match(url, pattern, options);
			var org = match.Groups["organization"].Value;
			var team = match.Groups["team"].Value;
			var owner = match.Groups["owner"].Value;
			var name = match.Groups["name"].Value;

			// Check if there are generic InterApp messages waiting on content creation
			var table = IAC_MessagesTable.GetTable(protocol);
			foreach (var iacRow in table.Rows.Where(iac => iac.ResponseType.AssemblyQualifiedName == typeof(AddRepositoryCollaboratorResponse).AssemblyQualifiedName))
			{
				var request = (GenericInterAppMessage<AddRepositoryCollaboratorRequest>)iacRow.Request;
				if (request.Data.Data.CollaboratorType != CollaboratorType.Team)
				{
					continue;
				}

				var iacOrg = iacRow.Info.Split('/')[0];
				var iacOwner = iacRow.Info.Split('/')[1];
				var iacName = iacRow.Info.Split('/')[2];
				var iacTeam = iacRow.Info.Split('/')[3];

				if (org == iacOrg &&
					team == iacTeam &&
					owner == iacOwner &&
					name == iacName)
				{
					var returnMessage = (GenericInterAppMessage<AddRepositoryCollaboratorResponse>)iacRow.Response;
					returnMessage.Data.Success = true;
					returnMessage.Data.RepositoryId = new RepositoryId(owner, name);
					returnMessage.Data.Description = $"Successfully added team '{team}'.";
					iacRow.Request.Reply(protocol.SLNet.RawConnection, returnMessage, Types.KnownTypes);
					iacRow.Status = IAC_MessageStatus.Confirmed;
					iacRow.SaveToProtocol(protocol);
				}
			}
		}
	}
}
