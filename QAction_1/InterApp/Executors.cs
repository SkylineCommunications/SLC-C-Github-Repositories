namespace Skyline.Protocol.InterApp
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.Protocol.InterApp.Executors.Repositories;
	using Skyline.Protocol.InterApp.Executors.Workflows;

	public static class Mapping
	{
		private static readonly IDictionary<Type, Type> InternalMessageToExecutorMapping = new Dictionary<Type, Type>
		{
			// Repositories
			{ typeof(GenericInterAppMessage<AddRepositoryRequest>),                     typeof(AddRepositoryExecutor) },
			{ typeof(GenericInterAppMessage<CreateRepositoryRequest>),                  typeof(CreateRepositoryExecutor) },
			{ typeof(GenericInterAppMessage<CreateRepositoryContentRequest>),           typeof(CreateRepositoryContentExecutor) },
			{ typeof(GenericInterAppMessage<AddRepositoryCollaboratorRequest>),         typeof(AddRepositoryCollaboratorExecutor) },
			{ typeof(GenericInterAppMessage<RemoveRepositoryRequest>),                  typeof(RemoveRepositoryExecutor) },
			{ typeof(GenericInterAppMessage<AddRepositoryTopicsRequest>),				typeof(AddRepositoryTopicsExecutor)},
			{ typeof(GenericInterAppMessage<RemoveRepositoryTopicsRequest>),            typeof(RemoveRepositoryTopicsExecutor)},

			// Workflows
			{ typeof(GenericInterAppMessage<AddAutomationScriptCIWorkflowRequest>),     typeof(AddAutomationScriptCIWorkflowExecutor) },
			{ typeof(GenericInterAppMessage<AddAutomationScriptCICDWorkflowRequest>),   typeof(AddAutomationScriptCICDWorkflowExecutor) },
			{ typeof(GenericInterAppMessage<AddConnectorCIWorkflowRequest>),            typeof(AddConnectorCIWorkflowExecutor) },
			{ typeof(GenericInterAppMessage<AddNugetCICDWorkflowRequest>),              typeof(AddNugetCICDWorkflowExecutor) },
			{ typeof(GenericInterAppMessage<AddInternalNugetCICDWorkflowRequest>),      typeof(AddInternalNugetCICDWorkflowExecutor) },
			{ typeof(GenericInterAppMessage<ExecuteWorkflowRequest>),					typeof(ExecuteWorkflowExecutor) },
		};

		public static IDictionary<Type, Type> MessageToExecutorMapping => InternalMessageToExecutorMapping;
	}
}
