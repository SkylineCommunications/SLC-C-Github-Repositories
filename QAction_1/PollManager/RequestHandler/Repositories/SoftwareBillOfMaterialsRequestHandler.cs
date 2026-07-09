namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using System.Linq;
	using Newtonsoft.Json;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleSoftwareBillOfMaterialsRequest(SLProtocol protocol, bool executeNow)
		{
			var rows = SLTables.Repositories.GetPrimaryKeys(protocol);
			var row = rows.Count > 0 ? rows[0] : null;

			if(row == null)
			{
				SLTables.PollManager.SetPollingStatus(protocol, RequestType.Repositories_SoftwareBillOfMaterials, PollingStatus.Idle);
				return;
			}

			var queue = rows.Where(r => r != row).ToList();
			protocol.SetParameter(Parameter.repositoriessoftwarebillofmaterialspollingqueue_2198, JsonConvert.SerializeObject(queue));
			HandleSoftwareBillOfMaterialsRequest(protocol, row, executeNow);
		}

		public static void HandleSoftwareBillOfMaterialsRequest(SLProtocol protocol, string repositoryId, bool executeNow)
		{
			protocol.SetParameter(Parameter.getrepositorysoftwarebillofmaterialsurl, $"repos/{repositoryId}/dependency-graph/sbom");
			var trigger = executeNow ? Triggers.GetRepositorySoftwareBillOfMaterialsNow : Triggers.GetRepositorySoftwareBillOfMaterials;
			protocol.CheckTrigger((int)trigger);
		}
	}
}
