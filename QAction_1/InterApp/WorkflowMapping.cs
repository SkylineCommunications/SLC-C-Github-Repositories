namespace Skyline.Protocol.InterApp
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages;
	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
	using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;

	internal static class WorkflowMapping
	{
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
