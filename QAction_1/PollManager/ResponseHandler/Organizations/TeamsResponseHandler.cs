namespace Skyline.Protocol.PollManager.ResponseHandler.Organizations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Organizations;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
	using Skyline.Protocol.API.Headers;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager.RequestHandler.Organizations;
	using Skyline.Protocol.Tables;

	using Extensions = Skyline.Protocol.Extensions.Extensions;

	public static partial class OrganizationsResponseHandler
	{
		public static void HandleOrganizationTeamsResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Organizations_Teams, PollingStatus.Idle);
				return;
			}

			// Parse response
			var response = SecureNewtonsoftDeserialization.DeserializeObject<List<Team>>(
				Convert.ToString(protocol.GetParameter(Parameter.getorganizationteamscontent)));
			if (response == null)
			{
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Organizations_Teams, PollingStatus.Idle);
				protocol.Log($"QA{protocol.QActionID}|HandleOrganizationTeamsResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			if (!response.Any())
			{
				// No repositories for the organization
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Organizations_Teams, PollingStatus.Idle);
				protocol.Log($"QA{protocol.QActionID}|HandleOrganizationTeamsResponse|No teams", LogType.Information, LogLevel.Level2);
				return;
			}

			// Parse url to check which respository this issue is linked to
			var pattern = "https:\\/\\/github.com\\/orgs\\/(?<owner>.*)\\/teams\\/(?<slug>.*)";
			var options = RegexOptions.Multiline;

			var utcNow = DateTime.UtcNow;
			var match = Regex.Match(response[0].HtmlUrl.OriginalString, pattern, options);
			var owner = match.Groups["owner"].Value;

			var rows = new List<OrganizationteamsQActionRow>();
			foreach (var team in response)
			{
				// Update existing organization if found, otherwise create new one
				var id = $"{owner}/{team.Slug}";
				var row = new TeamsModel
				{
					Instance = id,
					Id = team.Id,
					Organization = owner,
					Name = team.Name,
					Slug = team.Slug,
					Description = team.Description,
					Privacy = Extensions.ParseEnumDescription<PrivacySetting>(team.Privacy),
					NotificationsEnabled = Extensions.ParseEnumDescription<NotificationSetting>(team.NotificationSetting),
					Permission = team.Permission,
					LastPolledAt = utcNow,
				};

				rows.Add(TeamsRowConverter.Instance.ToRawValue(row));
			}

			if (rows.Count > 0)
			{
				SLTables.Teams.FillTableNoDelete(protocol, rows);
			}

			// Check if there are more repositories to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getorganizationteamslinkheader));
			var link = new LinkHeader(linkHeader);

			if (link.HasNext)
			{
				var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Organizations_Teams)?.PageLimit ?? PollingConstants.PerPage;
				OrganizationsRequestHandler.HandleOrganizationTeamsRequest(protocol, owner, perPage, link.NextPage, true);
			}
			else
			{
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Organizations_Teams, PollingStatus.Idle);
				SLTables.Teams.Cleanup(protocol, owner);
			}
		}
	}
}
