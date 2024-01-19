namespace Skyline.Protocol.PollManager.ResponseHandler.Organizations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    using Skyline.DataMiner.Scripting;
    using Skyline.DataMiner.Utils.Github.API.V20221128.Organizations;
    using Skyline.Protocol.Extensions;
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
            var avatarurl = response[0].AvatarUrl;
            protocol.Log($"QA{protocol.QActionID}|HandleUserOrganizationsResponse|avatarUrl: {avatarurl}", LogType.DebugInfo, LogLevel.NoLogging);
            protocol.Log($"QA{protocol.QActionID}|HandleUserOrganizationsResponse|avatarUrl == null: {avatarurl == null}", LogType.DebugInfo, LogLevel.NoLogging);

            var table = new OrganizationsTable(protocol);
            foreach(var org in response)
            {
                // Update existing organization if found, otherwise create new one
                var row = table.Rows.Find(organization => organization.Instance == org.Login) ?? new OrganizationsTableRow();
                row.Id = org.Id;
                row.Description = org.Description;
                row.AvatarURL = avatarurl;

                // If its a new row fill in ID and default values and add it to the table.
                if (String.IsNullOrEmpty(row.Instance))
                {
                    row.Instance = org.Login;
                    row.Tracked = false;
                    table.Rows.Add(row);
                }
            }

            protocol.SetParameter(Parameter.addorganizationrepositories_discreetlist_507, String.Join(";", table.Rows.Select(org => org.Instance)));
            table.SaveToProtocol(protocol);
        }
    }
}
