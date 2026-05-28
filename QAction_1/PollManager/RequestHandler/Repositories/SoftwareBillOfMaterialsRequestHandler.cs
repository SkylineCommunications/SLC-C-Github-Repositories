namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleSoftwareBillOfMaterialsRequest(SLProtocol protocol, bool executeNow)
		{
			var rows = SLTables.Repositories.GetPrimaryKeys(protocol);
			foreach (var row in rows)
			{
				HandleSoftwareBillOfMaterialsRequest(protocol, row, executeNow);
			}
		}

		public static void HandleSoftwareBillOfMaterialsRequest(SLProtocol protocol, string repositoryId, bool executeNow)
		{
			protocol.SetParameter(Parameter.getrepositorysoftwarebillofmaterialsurl, $"repos/{repositoryId}/dependency-graph/sbom");
			var trigger = executeNow ? Triggers.GetRepositorySoftwareBillOfMaterialsNow : Triggers.GetRepositorySoftwareBillOfMaterials;
			protocol.CheckTrigger((int)trigger);
		}
	}
}
