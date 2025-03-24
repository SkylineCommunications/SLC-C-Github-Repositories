// Ignore Spelling: App Workflows

namespace Skyline.Protocol.InterApp.Executors.Workflows
{
    using System;

    using Skyline.DataMiner.ConnectorAPI.Github.Repositories;
    using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;
    using Skyline.DataMiner.Core.InterAppCalls.Common.CallSingle;
    using Skyline.Protocol.Tables;

    internal static class WorkflowValidation
    {
        public static bool Validate(AddWorkflowRequest workflowRequest, RepositoriesTableRow repo, out string result)
        {
            result = String.Empty;

            if (String.IsNullOrWhiteSpace(workflowRequest.RepositoryId.Owner) ||
                String.IsNullOrWhiteSpace(workflowRequest.RepositoryId.Name))
            {
                result = "The Owner and Name of the repository cannot be left empty.";
                return false;
            }

            // Check if the repository exists in the connector
            if (repo == default)
            {
                result = $"The given repository '{workflowRequest.RepositoryId.FullName}', is not tracked by this element";
                return false;
            }

            // Check if the public keys are fetched
            if (repo.PublicKey == Exceptions.NotAvailable ||
                repo.PublicKeyID == Exceptions.NotAvailable)
            {
                result = $"The public keys are not available for '{workflowRequest.RepositoryId.FullName}'. Either the configured API Token does not have permission to the repository, or the public keys for the repository are not fetched yet.";
                return false;
            }

            var success = false;
            switch (workflowRequest)
            {
                case AddAutomationScriptCICDWorkflowRequest automationScriptCICDWorkflowRequest:
                    success = ValidateAutomationScriptCICD(automationScriptCICDWorkflowRequest, out result);
                    break;

                case AddAutomationScriptCIWorkflowRequest automationScriptCIWorkflowRequest:
                    success = ValidateAutomationScriptCI(automationScriptCIWorkflowRequest, out result);
                    break;

                case AddConnectorCIWorkflowRequest connectorCIWorkflowRequest:
                    success = ValidateConnectorCI(connectorCIWorkflowRequest, out result);
                    break;

                case AddInternalNugetCICDWorkflowRequest internalNugetCICDWorkflowRequest:
                    success = ValidateInternalNugetCICD(internalNugetCICDWorkflowRequest, out result);
                    break;

                case AddNugetCICDWorkflowRequest nugetCICDWorkflowRequest:
                    success = ValidateNugetCICD(nugetCICDWorkflowRequest, out result);
                    break;

                default:
                    result = "Invalid InterApp Request";
                    return false;
            }

            if (!success)
            {
                return false;
            }

            return true;
        }

        public static bool Validate(ExecuteWorkflowRequest workflowRequest, RepositoriesTableRow repo, out string result)
        {
            result = String.Empty;

            if (String.IsNullOrWhiteSpace(workflowRequest.RepositoryId.Owner) ||
                String.IsNullOrWhiteSpace(workflowRequest.RepositoryId.Name))
            {
                result = "The Owner and Name of the repository cannot be left empty.";
                return false;
            }

            // Check if the repository exists in the connector
            if (repo == default)
            {
                result = $"The given repository '{workflowRequest.RepositoryId.FullName}', is not tracked by this element";
                return false;
            }

            return true;
        }

        private static bool ValidateAutomationScriptCICD(AddAutomationScriptCICDWorkflowRequest workflowRequest, out string result)
        {
            result = String.Empty;

            // Check sonarcloud project id
            if (String.IsNullOrWhiteSpace(workflowRequest.Data.SonarCloudProjectID))
            {
                result = "The sonar cloud project id cannot be left blank. Go to https://sonarcloud.io/ to retrieve the id.";
                return false;
            }

            // Check dataminer deploy key
            if (String.IsNullOrWhiteSpace(workflowRequest.Data.DataMinerKey))
            {
                result = "The DataMiner Deploy key cannot be left blank. Go to https://admin.dataminer.services/ the get one.";
                return false;
            }

            return true;
        }

        private static bool ValidateAutomationScriptCI(AddAutomationScriptCIWorkflowRequest workflowRequest, out string result)
        {
            result = String.Empty;

            // Check sonarcloud project id
            if (String.IsNullOrWhiteSpace(workflowRequest.Data.SonarCloudProjectID))
            {
                result = "The sonar cloud project id cannot be left blank. Go to https://sonarcloud.io/ to retrieve the id.";
                return false;
            }

            // Check dataminer deploy key
            if (String.IsNullOrWhiteSpace(workflowRequest.Data.DataMinerKey))
            {
                result = "The DataMiner Deploy key cannot be left blank. Go to https://admin.dataminer.services/ the get one.";
                return false;
            }

            return true;
        }

        private static bool ValidateConnectorCI(AddConnectorCIWorkflowRequest workflowRequest, out string result)
        {
            result = String.Empty;

            // Check sonarcloud project id
            if (String.IsNullOrWhiteSpace(workflowRequest.Data.SonarCloudProjectID))
            {
                result = "The sonar cloud project id cannot be left blank. Go to https://sonarcloud.io/ to retrieve the id.";
                return false;
            }

            // Check dataminer deploy key
            if (String.IsNullOrWhiteSpace(workflowRequest.Data.DataMinerKey))
            {
                result = "The DataMiner Deploy key cannot be left blank. Go to https://admin.dataminer.services/ the get one.";
                return false;
            }

            return true;
        }

        private static bool ValidateInternalNugetCICD(AddInternalNugetCICDWorkflowRequest workflowRequest, out string result)
        {
            result = String.Empty;

            // Check sonarcloud project id
            if (String.IsNullOrWhiteSpace(workflowRequest.Data.SonarCloudProjectID))
            {
                result = "The sonar cloud project id cannot be left blank. Go to https://sonarcloud.io/ to retrieve the id.";
                return false;
            }

            // Check Github API key
            if (String.IsNullOrWhiteSpace(workflowRequest.Data.GithubNugetApiKey))
            {
                result = "The Github API Key cannot be left blank.";
                return false;
            }

            return true;
        }

        private static bool ValidateNugetCICD(AddNugetCICDWorkflowRequest workflowRequest, out string result)
        {
            result = String.Empty;

            // Check sonarcloud project id
            if (String.IsNullOrWhiteSpace(workflowRequest.Data.SonarCloudProjectID))
            {
                result = "The sonar cloud project id cannot be left blank. Go to https://sonarcloud.io/ to retrieve the id.";
                return false;
            }

            // Check Nuget API key
            if (String.IsNullOrWhiteSpace(workflowRequest.Data.NugetApiKey))
            {
                result = "The Nuget API Key cannot be left blank.";
                return false;
            }

            return true;
        }
    }
}
