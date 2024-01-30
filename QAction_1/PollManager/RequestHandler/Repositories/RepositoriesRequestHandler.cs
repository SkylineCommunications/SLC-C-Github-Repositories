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
		public static void HandleRepositoriesRequest(SLProtocol protocol)
		{
			// Retrieve tracked organizations
			var table = RepositoriesTable.GetTable(protocol);
			var trackedOrganizations = new List<string>();
			var pollmanager = PollManager.InitPollDictionary(protocol);
			if (pollmanager[RequestType.Organizations_Repositories].Enabled)
			{
				var orgs = OrganizationsTable.GetTable(protocol);
				foreach(var org in orgs.Rows)
				{
					if (!org.Tracked)
						continue;

					trackedOrganizations.Add(org.Instance);
				}
			}

			// Fetch all the repositories that are not fetch throught the organization.
			foreach (var row in table.Rows.Where(repo => !trackedOrganizations.Contains(repo.Owner)))
			{
				protocol.SetParameter(Parameter.getrepositoryurl, $"repos/{row.Owner}/{row.Name}");
				protocol.CheckTrigger(201);
			}
		}

		public static void CreateRepositoryContent(SLProtocol protocol, string repositoryId, string path, string content, string commitMessage)
		{
			var body = new PutRepositoryContent
			{
				Message = commitMessage,
				Content = content.Base64Encode(),
			};

			var encodedPath = String.Join("/", path.Split('/').Select(part => HttpUtility.UrlEncode(part)));
			var sets = new Dictionary<int, object>
			{
				{ Parameter.putrepositorycontenturl,		$"repos/{repositoryId}/contents/{encodedPath}" },
				{ Parameter.putrepositorycontentbody,		JsonConvert.SerializeObject(body) },
			};

			protocol.SetParameters(sets.Keys.ToArray(), sets.Values.ToArray());
			protocol.CheckTrigger(221);
		}
	}
}
