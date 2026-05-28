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
	using Skyline.Protocol.PollManager.RequestHandler.Repositories;
	using Skyline.Protocol.Tables;

	using Extensions = Skyline.Protocol.Extensions.Extensions;

	public static partial class OrganizationsResponseHandler
	{
		public static void HandleOrganizationMembersResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = SecureNewtonsoftDeserialization.DeserializeObject<List<Member>>(
				Convert.ToString(protocol.GetParameter(Parameter.getorganizationmemberscontent)));
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
			var url = Convert.ToString(protocol.GetParameter(Parameter.getorganizationmembersurl));
			var pattern = "orgs\\/(?<Organization>.*)\\/members";
			var options = RegexOptions.Multiline;

			var utcNow = DateTime.UtcNow;
			var match = Regex.Match(url, pattern, options);
			var org = match.Groups["Organization"].Value;

			var memberRows = new List<OrganizationmembersQActionRow>();
			var memberLinks = new List<MemberorganizationlinksQActionRow>();
			foreach (var member in response)
			{
				// Update existing member if found, otherwise create new one
				var row = new MembersModel();
				row.Instance = member.Login;
				row.Id = member.Id;
				row.Login = member.Login;
				row.Type = member.Type;
				row.SiteAdmin = member.SiteAdmin;
				row.Url = member.Url;
				row.HtmlUrl = member.HtmlUrl;
				row.AvatarUrl = member.AvatarUrl;
				row.LastPolledAt = utcNow;

				memberRows.Add(MembersRowConverter.Instance.ToRawValue(row));

				// Update the members to organization linker table
				var linkerId = $"{org}/{member.Login}";
				var linkerRow = new MemberOrganizationLinksModel();
				if (SLTables.MemberOrganizationLinks.TryGetRow(protocol, linkerId, out var existingOrgLink))
				{
					linkerRow = MemberOrganizationLinksRowConverter.Instance.FromRawValue(existingOrgLink);
				}

				linkerRow.Instance = linkerId;
				linkerRow.Organization = org;
				linkerRow.Member = member.Login;
				linkerRow.LastPolledAt = utcNow;

				memberLinks.Add(MemberOrganizationLinksRowConverter.Instance.ToRawValue(linkerRow));
			}

			if (memberRows.Count > 0)
			{
				SLTables.Members.FillTableNoDelete(protocol, memberRows);
			}

			if (memberLinks.Count > 0)
			{
				SLTables.MemberOrganizationLinks.FillTableNoDelete(protocol, memberLinks);
			}

			// Check if there are more repositories to fetch
			var linkHeader = Convert.ToString(protocol.GetParameter(Parameter.getorganizationmemberslinkheader));
			var link = new LinkHeader(linkHeader);
			if (link.HasNext)
			{
				OrganizationsRequestHandler.HandleOrganizationMembersRequest(protocol, org, link.NextPage, true);
			}
			else
			{
				SLTables.Members.Cleanup(protocol);
				SLTables.MemberOrganizationLinks.Cleanup(protocol, org);
			}
		}
	}
}
