namespace Skyline.Protocol.PollManager.ResponseHandler.Organizations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Organizations;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
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
			var response = SecureNewtonsoftDeserialization.DeserializeObject<List<Organization>>(
				Convert.ToString(protocol.GetParameter(Parameter.getuserorganizationscontent_210)));
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

			var rows = new List<OrganizationsQActionRow>();
			foreach (var org in response)
			{
				// Update existing organization if found, otherwise create new one
				var row = new OrganizationsModel();
				if (SLTables.Organizations.TryGetRow(protocol, org.Login, out var rawRow))
				{
					row = OrganizationsRowConverter.Instance.FromRawValue(rawRow);
				}

				row.Id = org.Id;
				row.Description = org.Description;
				row.AvatarUrl = org.AvatarUrl.OriginalString;

				// If its a new row fill in ID and default values and add it to the table.
				if (String.IsNullOrEmpty(row.Instance))
				{
					row.Instance = org.Login;
					row.Tracked = false;
				}

				rows.Add(OrganizationsRowConverter.Instance.ToRawValue(row));
			}

			if (rows.Count > 0)
			{
				SLTables.Organizations.FillTableNoDelete(protocol, rows);
			}

			// Check if there are more repositories to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getuserorganizationslinkheader));
			var link = new LinkHeader(linkHeader);

			protocol.Log($"QA{protocol.QActionID}|HandleUserOrganizationsResponse|Current page: {link.CurrentPage}", LogType.Information, LogLevel.Level2);
			protocol.Log($"QA{protocol.QActionID}|HandleUserOrganizationsResponse|Has next page: {link.HasNext}", LogType.Information, LogLevel.Level2);

			if (link.HasNext)
			{
				var perPage = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Organizations_User)?.PageLimit ?? PollingConstants.PerPage;
				OrganizationsRequestHandler.HandleUserOrganizationsRequest(protocol, perPage, link.NextPage, true);
			}
		}
	}
}
