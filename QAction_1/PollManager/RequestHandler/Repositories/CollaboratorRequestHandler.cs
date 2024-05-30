namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Extensions;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleOrganizationAddRepositoryCollaborator(SLProtocol protocol, string owner, string name, string userSlug, PermissionType permission = PermissionType.Read)
		{
			var settings = new JsonSerializerSettings
			{
				NullValueHandling = NullValueHandling.Ignore,
			};

			var sets = new Dictionary<int, object>
			{
				{ Parameter.putrepositoryusercollaboratorurl,              $"repos/{owner}/{name}/collaborators/{userSlug}" },
				{ Parameter.putrepositoryusercollaboratorbody,             JsonConvert.SerializeObject(new { permission = permission.GetGithubPermission() }, settings) },
			};

			protocol.SetParameters(sets.Keys.ToArray(), sets.Values.ToArray());
			protocol.CheckTrigger(223);
		}
	}
}
