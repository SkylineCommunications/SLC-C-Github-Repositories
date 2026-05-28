// Ignore Spelling: Workflows Workflow

namespace Skyline.Protocol.PollManager.RequestHandler.Repositories
{
	using System;
	using System.Reflection;
	using System.Text;

	using Newtonsoft.Json;

	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.API.Secrets;
	using Skyline.Protocol.API.Variables;

	public static partial class RepositoriesRequestHandler
	{
		public static void CreateRepositoryVariable(SLProtocol protocol, string repositoryId, string variableName, string value)
		{
			var body = new VariablePair
			{
				Name = variableName,
				Value = value,
			};

			protocol.SetParameter(Parameter.postrepositoryvariableurl_135, $"repos/{repositoryId}/actions/variables");
			protocol.SetParameter(Parameter.postrepositoryvariablebody_185, JsonConvert.SerializeObject(body));
			protocol.CheckTrigger((int)Triggers.PostRepositoryVariableNow);
		}
	}
}
