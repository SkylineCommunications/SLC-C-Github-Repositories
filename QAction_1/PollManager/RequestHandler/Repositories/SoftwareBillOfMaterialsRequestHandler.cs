namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleSoftwareBillOfMaterialsRequest(SLProtocol protocol)
		{
			var rows = SLTables.Repositories.GetPrimaryKeys(protocol);
			foreach (var row in rows)
			{
				HandleSoftwareBillOfMaterialsRequest(protocol, row);
			}
		}

		public static void HandleSoftwareBillOfMaterialsRequest(SLProtocol protocol, string repositoryId)
		{
			protocol.SetParameter(Parameter.getrepositorysoftwarebillofmaterialsurl, $"repos/{repositoryId}/dependency-graph/sbom");
			protocol.CheckTrigger(206);
		}
	}
}
