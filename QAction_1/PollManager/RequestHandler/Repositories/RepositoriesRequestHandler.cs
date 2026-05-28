namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Web;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.API.Content;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesRequest(SLProtocol protocol, bool executeNow)
		{
			// Retrieve tracked organizations
			var trackedOrganizations = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			var pollRow = SLTables.PollManager.GetRowByRequestType(protocol, RequestType.Organizations_Repositories);
			if (pollRow?.PollState == PollState.Enabled)
			{
				trackedOrganizations = SLTables.Organizations.GetData(
					protocol,
					SLTables.Organizations.Instance.Read.Map<OrganizationsModel>(m => m.Instance),
					SLTables.Organizations.Tracked.Read.Map<OrganizationsModel>(m => m.Tracked))
						.Select(m => new { m.Instance, m.Tracked })
						.Where(m => m.Tracked.HasValue && m.Tracked.Value)
						.Select(m => m.Instance)
						.ToHashSet();
			}

			// Fetch all the repositories that are not fetch through the organization.
			var repositories = SLTables.Repositories.GetData(protocol,
				SLTables.Repositories.FullName.Read.Map<RepositoriesModel>(m => m.FullName),
				SLTables.Repositories.Owner.Read.Map<RepositoriesModel>(m => m.Owner))
				.Select(m => new { m.FullName, m.Owner })
				.ToArray();
			foreach (var row in repositories.Where(repo => !trackedOrganizations.Contains(repo.Owner)))
			{
				HandleRepositoriesRequest(protocol, row.FullName, executeNow);
			}
		}

		public static void HandleRepositoriesRequest(SLProtocol protocol, string repositoryId, bool executeNow)
		{
			protocol.SetParameter(Parameter.getrepositoryurl, $"repos/{repositoryId}");
			var trigger = executeNow ? Triggers.GetRepositoryNow : Triggers.GetRepository;
			protocol.CheckTrigger((int)trigger);
		}

		public static void CreateRepositoryContent(SLProtocol protocol, string repositoryId, string path, string content, string commitMessage)
		{
			var body = new PutRepositoryContentRequest
			{
				Message = commitMessage,
				Content = content.Base64Encode(),
			};

			var encodedPath = String.Join("/", path.Split('/').Select(part => HttpUtility.UrlEncode(part)));
			var sets = new Dictionary<int, object>
			{
				{ Parameter.putrepositorycontenturl,        $"repos/{repositoryId}/contents/{encodedPath}" },
				{ Parameter.putrepositorycontentbody,       JsonConvert.SerializeObject(body) },
			};

			protocol.SetParameters(sets.Keys.ToArray(), sets.Values.ToArray());
			protocol.CheckTrigger((int)Triggers.PutRepositoryContentNow);
		}
	}
}
