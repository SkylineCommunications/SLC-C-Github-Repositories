namespace Skyline.Protocol.InterApp
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.Protocol.InterApp.Executors.Repositories;
	using Skyline.Protocol.InterApp.Executors.Workflows;

	public static class Mapping
	{
		private static readonly IDictionary<Type, Type> InternalMessageToExecutorMapping = new Dictionary<Type, Type>
		{
			// Repositories
			{ typeof(AddRepositoryRequest),						typeof(AddRepositoryExecutor) },
			{ typeof(RemoveRepositoryRequest),					typeof(RemoveRepositoryExecutor) },

			// Workflows
			{ typeof(AddAutomationScriptCIWorkflowRequest),		typeof(AddAutomationScriptCIWorkflowExecutor) },
			{ typeof(AddAutomationScriptCICDWorkflowRequest),	typeof(AddAutomationScriptCICDWorkflowExecutor) },
			{ typeof(AddConnectorCIWorkflowRequest),			typeof(AddConnectorCIWorkflowExecutor) },
			{ typeof(AddNugetCICDWorkflowRequest),				typeof(AddNugetCICDWorkflowExecutor) },
		};

		public static IDictionary<Type, Type> MessageToExecutorMapping => InternalMessageToExecutorMapping;
	}
}
