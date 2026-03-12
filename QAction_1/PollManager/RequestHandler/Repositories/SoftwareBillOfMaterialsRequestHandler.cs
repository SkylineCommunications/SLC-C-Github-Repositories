namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesRequestHandler
	{
		public static void HandleSoftwareBillOfMaterialsRequest(SLProtocol protocol)
		{
			var table = RepositoriesTable.GetTable(protocol);
			foreach (var row in table.Rows)
			{
				HandleSoftwareBillOfMaterialsRequest(protocol, row.Owner, row.Name);
			}
		}

		public static void HandleSoftwareBillOfMaterialsRequest(SLProtocol protocol, string owner, string name)
		{
			protocol.SetParameter(Parameter.getrepositorysoftwarebillofmaterialsurl, $"repos/{owner}/{name}/dependency-graph/sbom");
			protocol.CheckTrigger(206);
		}
	}
}
