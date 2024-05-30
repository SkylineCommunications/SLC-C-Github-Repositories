namespace Skyline.Protocol.PollManager.ResponseHandler.Organizations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Github.API.V20221128.Organizations;
	using Skyline.Protocol.API.Headers;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.PollManager.RequestHandler.Organizations;
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	using Extensions = Skyline.Protocol.Extensions.Extensions;

	public static partial class OrganizationsResponseHandler
	{
		public static void HandleOrganizationMembersResponse(SLProtocol protocol)
		{
			// Check status code
			var code = protocol.GetStatusCode();
			if (code >= 400 && code <= 499)
			{
				HandleNextOrganizationMembers(protocol);
				return;
			}

			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = JsonConvert.DeserializeObject<List<Member>>(Convert.ToString(protocol.GetParameter(Parameter.getorganizationmemberscontent)));
			var url = Convert.ToString(protocol.GetParameter(Parameter.getorganizationmembersurl));

			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|HandleOrganizationMembersResponse|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			if (!response.Any())
			{
				// No repositories for the organization
				protocol.Log($"QA{protocol.QActionID}|HandleOrganizationMembersResponse|No Members", LogType.Information, LogLevel.Level2);
				return;
			}

			// Parse url to check which organization this member is linked to
			var pattern = "orgs\\/(?<Organization>.*)\\/members";
			var options = RegexOptions.Multiline;

			var match = Regex.Match(url, pattern, options);
			var org = match.Groups["Organization"].Value;

			var table = MembersTable.GetTable();
			var linkerTable = MemberOrganizationLinksTable.GetTable();
			foreach (var member in response)
			{
				// Update existing member if found, otherwise create new one
				var id = $"{org}/{member.Login}";
				var row = table.Rows.Find(t => t.Instance == member.Login) ?? new MembersTableRow();
				row.Id = member.Id;
				row.Login = member.Login;
				row.Type = member.Type;
				row.SiteAdmin = member.SiteAdmin;
				row.Url = member.Url;
				row.HtmlUrl = member.HtmlUrl;
				row.AvatarUrl = member.AvatarUrl;

				// If its a new row fill in ID and add it to the table.
				if (row.Instance == Exceptions.NotAvailable)
				{
					row.Instance = member.Login;
					table.Rows.Add(row);
				}

				// Update the members to organization linker table
				var linkerRow = linkerTable.Rows.Find(l => l.Instance == id) ?? new MemberOrganizationLinksTableRow();
				linkerRow.Organization = org;
				linkerRow.Member = member.Login;

				// If its a new row fill in ID and add it to the table.
				if (linkerRow.Instance == Exceptions.NotAvailable)
				{
					linkerRow.Instance = id;
					linkerTable.Rows.Add(linkerRow);
				}
			}

			if (table.Rows.Count > 0)
			{
				table.SaveToProtocol(protocol, true);
			}

			if (linkerTable.Rows.Count > 0)
			{
				linkerTable.SaveToProtocol(protocol, true);
			}

			// Check if there are more repositories to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getorganizationmemberslinkheader));
			if (string.IsNullOrEmpty(linkHeader))
			{
				HandleNextOrganizationMembers(protocol);
			}

			var link = new LinkHeader(linkHeader);

			if (link.HasNext)
			{
				OrganizationsRequestHandler.HandleOrganizationMembersRequest(protocol, org, PollingConstants.PerPage, link.NextPage);
			}
			else
			{
				HandleNextOrganizationMembers(protocol);
			}
		}

		private static void HandleNextOrganizationMembers(SLProtocol protocol)
		{
			// Get the next repo in the queue to fetch
			var queue = JsonConvert.DeserializeObject<List<string>>(Convert.ToString(protocol.GetParameter(Parameter.getorganizationmembersqueue)));
			var next = queue?.FirstOrDefault();

			if (next == null)
			{
				return;
			}

			protocol.SetParameter(Parameter.getorganizationmembersqueue, JsonConvert.SerializeObject(queue.Skip(1)));
			OrganizationsRequestHandler.HandleOrganizationMembersRequest(protocol, next, PollingConstants.PerPage, 1);
		}
	}
}
