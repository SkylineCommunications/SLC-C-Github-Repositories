namespace Skyline.Protocol.PollManager.RequestHandler.Organizations
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Extensions;

	public static partial class OrganizationsRequestHandler
	{
		public static void HandleOrganizationAddRepositoryCollaborator(SLProtocol protocol, string organization, string owner, string name, string teamSlug, PermissionType permission = PermissionType.Read)
		{
			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
			};

			var sets = new Dictionary<int, object>
			{
				{ Parameter.putrepositoryteamcollaboratorurl,              $"orgs/{organization}/teams/{teamSlug}/repos/{owner}/{name}" },
				{ Parameter.putrepositoryteamcollaboratorbody,             JsonConvert.SerializeObject(new { permission = permission.GetGithubPermission() }, settings) },
			};

			protocol.SetParameters(sets.Keys.ToArray(), sets.Values.ToArray());
			protocol.CheckTrigger((int)Triggers.PutRepositoryTeamCollaboratorNow);
		}
	}
}
