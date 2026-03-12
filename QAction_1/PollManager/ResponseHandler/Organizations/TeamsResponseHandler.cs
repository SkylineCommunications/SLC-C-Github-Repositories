namespace Skyline.Protocol.PollManager.ResponseHandler.Organizations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	using Newtonsoft.Json;

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
				return;
			}

			// Parse response
			var response = SecureNewtonsoftDeserialization.DeserializeObject<List<Team>>(
				Convert.ToString(protocol.GetParameter(Parameter.getorganizationteamscontent)));
			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|HandleOrganizationTeamsResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			if (!response.Any())
			{
				// No repositories for the organization
				protocol.Log($"QA{protocol.QActionID}|HandleOrganizationTeamsResponse|No teams", LogType.Information, LogLevel.Level2);
				return;
			}

			// Parse url to check which respository this issue is linked to
			var pattern = "https:\\/\\/github.com\\/orgs\\/(?<owner>.*)\\/teams\\/(?<slug>.*)";
			var options = RegexOptions.Multiline;

			var utcNow = DateTime.UtcNow;
			var match = Regex.Match(response[0].HtmlUrl.OriginalString, pattern, options);
			var owner = match.Groups["owner"].Value;

			var table = TeamsTable.GetTable();
			foreach (var team in response)
			{
				// Update existing organization if found, otherwise create new one
				var id = $"{owner}/{team.Slug}";
				var row = table.Rows.Find(t => t.Instance == id) ?? new TeamsTableRow();
				row.Id = team.Id;
				row.Organization = owner;
				row.Name = team.Name;
				row.Slug = team.Slug;
				row.Description = team.Description;
				row.Privacy = Extensions.ParseEnumDescription<PrivacySetting>(team.Privacy);
				row.NotificationsEnabled = Extensions.ParseEnumDescription<NotificationSetting>(team.NotificationSetting);
				row.Permission = team.Permission;
				row.LastPolledAt = utcNow;

				// If its a new row fill in ID and add it to the table.
				if (row.Instance == Exceptions.NotAvailable)
				{
					row.Instance = id;
					table.Rows.Add(row);
				}
			}

			if (table.Rows.Count > 0)
			{
				table.SaveToProtocol(protocol, true);
			}

			// Check if there are more repositories to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getorganizationteamslinkheader));
			if (string.IsNullOrEmpty(linkHeader))
			{
				TeamsTable.GetTable(protocol).Cleanup(protocol, owner);
				return;
			}

			var link = new LinkHeader(linkHeader);

			if (link.HasNext)
			{
				OrganizationsRequestHandler.HandleOrganizationTeamsRequest(protocol, owner, PollingConstants.PerPage, link.NextPage);
			}
			else
			{
				TeamsTable.GetTable(protocol).Cleanup(protocol, owner);
			}
		}
	}
}
