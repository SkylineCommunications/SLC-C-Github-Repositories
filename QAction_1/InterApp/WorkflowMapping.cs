namespace Skyline.Protocol.InterApp
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows.Data;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	public static class WorkflowMapping
	{
		public static Message CreateEmptyWorkflowMessage(WorkflowType type, string owner, string name)
		{
			Message request = default;
			switch (type)
			{
				case WorkflowType.AutomationScriptCI:
					request = new GenericInterAppMessage<AddAutomationScriptCIWorkflowRequest>(
						new AddAutomationScriptCIWorkflowRequest
						{
							RepositoryId = new RepositoryId(owner, name),
							Data = new AutomationScriptCIWorkflowData
							{
								SonarCloudProjectID = String.Empty,
								DataMinerKey = String.Empty,
							},
						});
					break;

				case WorkflowType.AutomationScriptCICD:
					request = new GenericInterAppMessage<AddAutomationScriptCICDWorkflowRequest>(
						new AddAutomationScriptCICDWorkflowRequest
						{
							RepositoryId = new RepositoryId(owner, name),
							Data = new AutomationScriptCICDWorkflowData
							{
								SonarCloudProjectID = String.Empty,
								DataMinerKey = String.Empty,
							},
						});
					break;

				case WorkflowType.ConnectorCI:
					request = new GenericInterAppMessage<AddConnectorCIWorkflowRequest>(
						new AddConnectorCIWorkflowRequest
						{
							RepositoryId = new RepositoryId(owner, name),
							Data = new ConnectorCIWorkflowData
							{
								SonarCloudProjectID = String.Empty,
								DataMinerKey = String.Empty,
							},
						});
					break;

				case WorkflowType.NugetSolutionCICD:
					request = new GenericInterAppMessage<AddNugetCICDWorkflowRequest>(
						new AddNugetCICDWorkflowRequest
						{
							RepositoryId = new RepositoryId(owner, name),
							Data = new NugetCICDWorkflowData
							{
								SonarCloudProjectID = String.Empty,
								NugetApiKey = String.Empty,
							},
						});
					break;

				default:
					return default;
			}

			return request;
		}

		internal static AddWorkflowRequest FromMessage(Message message)
		{
			switch(message)
			{
				case GenericInterAppMessage<AddAutomationScriptCIWorkflowRequest> addAutomationScriptCIWorkflowRequest:
					return addAutomationScriptCIWorkflowRequest.Data;

				case GenericInterAppMessage<AddAutomationScriptCICDWorkflowRequest> addAutomationScriptCICDWorkflowRequest:
					return addAutomationScriptCICDWorkflowRequest.Data;

				case GenericInterAppMessage<AddConnectorCIWorkflowRequest> addConnectorCIWorkflowRequest:
					return addConnectorCIWorkflowRequest.Data;

				case GenericInterAppMessage<AddNugetCICDWorkflowRequest> addNugetCICDWorkflowRequest:
					return addNugetCICDWorkflowRequest.Data;

				case GenericInterAppMessage<AddInternalNugetCICDWorkflowRequest> addInternalNugetCICDWorkflowRequest:
					return addInternalNugetCICDWorkflowRequest.Data;

				default:
					throw new InvalidOperationException("Unknown workflow type");
			}
		}
	}
}
