namespace Skyline.Protocol.PollManager.ResponseHandler.Organizations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Organizations;
	using Skyline.Protocol.API.Headers;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager.RequestHandler.Organizations;
	using Skyline.Protocol.Tables;

	public static partial class OrganizationsResponseHandler
	{
		public static void HandleUserOrganizationsResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = JsonConvert.DeserializeObject<List<Organization>>(Convert.ToString(protocol.GetParameter(Parameter.getuserorganizationscontent_210)));
			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|HandleUserOrganizationsResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			if (!response.Any())
			{
				// No organizations for the user
				protocol.Log($"QA{protocol.QActionID}|HandleUserOrganizationsResponse|No organizations", LogType.Information, LogLevel.Level2);
				return;
			}

			var table = OrganizationsTable.GetTable(protocol);
			foreach (var org in response)
			{
				// Update existing organization if found, otherwise create new one
				var row = table.Rows.Find(organization => organization.Instance == org.Login) ?? new OrganizationsTableRow();
				row.Id = org.Id;
				row.Description = org.Description;
				row.AvatarURL = org.AvatarUrl;

				// If its a new row fill in ID and default values and add it to the table.
				if (String.IsNullOrEmpty(row.Instance))
				{
					row.Instance = org.Login;
					row.Tracked = false;
					table.Rows.Add(row);
				}
			}

			if(table.Rows.Count > 0)
			{
				protocol.SetParameter(Parameter.addorganizationrepositories_discreetlist_507, String.Join(";", table.Rows.Select(org => org.Instance)));
				table.SaveToProtocol(protocol);
			}

			// Check if there are more repositories to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getuserorganizationslinkheader));
			if (string.IsNullOrEmpty(linkHeader)) return;

			var link = new LinkHeader(linkHeader);

			protocol.Log($"QA{protocol.QActionID}|HandleUserOrganizationsResponse|Current page: {link.CurrentPage}", LogType.Information, LogLevel.Level2);
			protocol.Log($"QA{protocol.QActionID}|HandleUserOrganizationsResponse|Has next page: {link.HasNext}", LogType.Information, LogLevel.Level2);

			if (link.HasNext)
			{
				OrganizationsRequestHandler.HandleUserOrganizationsRequest(protocol, PollingConstants.PerPage, link.NextPage);
			}
		}
	}
}
