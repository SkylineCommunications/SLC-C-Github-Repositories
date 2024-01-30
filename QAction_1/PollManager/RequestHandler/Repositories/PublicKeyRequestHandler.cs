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
			var table = RepositoriesTable.GetTable(protocol);
			var first = table.Rows[0];
			protocol.SetParameter(Parameter.getrepositorypublickeyqueue, JsonConvert.SerializeObject(table.Rows.Select(x => x.FullName).Skip(1)));
			HandleRepositoriesPublicKeysRequest(protocol, first.Owner, first.Name);
		}

		public static void HandleRepositoriesPublicKeysRequest(SLProtocol protocol, string owner, string name)
		{
			protocol.SetParameter(Parameter.getrepositorypublickeyurl, $"repos/{owner}/{name}/actions/secrets/public-key");
			protocol.CheckTrigger(228);
		}
	}
}
