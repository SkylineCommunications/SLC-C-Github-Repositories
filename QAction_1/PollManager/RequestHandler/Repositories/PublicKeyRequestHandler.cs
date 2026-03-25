namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleRepositoriesPublicKeysRequest(SLProtocol protocol)
		{
			var repositories = SLTables.Repositories.GetPrimaryKeys(protocol);
			if (!repositories.Any())
			{
				return;
			}

			protocol.SetParameter(Parameter.getrepositorypublickeyqueue, JsonConvert.SerializeObject(repositories.Skip(1)));
			HandleRepositoriesPublicKeysRequest(protocol, repositories[0]);
		}

		public static void HandleRepositoriesPublicKeysRequest(SLProtocol protocol, string repositoryId)
		{
			protocol.SetParameter(Parameter.getrepositorypublickeyurl, $"repos/{repositoryId}/actions/secrets/public-key");
			protocol.CheckTrigger(228);
		}
	}
}
