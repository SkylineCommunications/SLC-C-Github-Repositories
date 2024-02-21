// Ignore Spelling: Workflow Workflows API sonarcloud Nuget

namespace Skyline.Protocol.API.Workflows
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.ConnectorAPI.Github.Repositories.InterAppMessages.Workflows;

	public static class WorkflowFactory
	{
		public static Workflow Create(WorkflowType workflowType)
		{
			switch (workflowType)
			{
				case WorkflowType.AutomationScriptCI:
					return CreateAutomationCIWorkflow();

				case WorkflowType.AutomationScriptCICD:
					return CreateAutomationCICDWorkflow();

				case WorkflowType.ConnectorCI:
					return CreateConnectorCIWorkflow();

				case WorkflowType.NugetSolutionCICD:
					return CreateNugetCICDWorkflow();

				case WorkflowType.InternalNugetSolutionCICD:
					return CreateInternalNugetCICDWorkflow();

				default:
					throw new NotSupportedException("The current workflow type is not supported yet");
			}
		}

		public static Workflow CreateAutomationCIWorkflow() => CreateAutomationCIWorkflow("# Grab your project id from https://sonarcloud.io/project/create.");

		public static Workflow CreateAutomationCIWorkflow(string sonarcloudProjectName)
		{
			var flow = new Workflow
			{
				Name = "DataMiner CI Automation",
				On = new On
				{
					Push = new Push
					{
						Branches = new List<string>(),
						Tags = new List<string>
						{
							"[0-9]+.[0-9]+.[0-9]+.[0-9]+",
							"[0-9]+.[0-9]+.[0-9]+-[0-9a-zA-Z]+",
						},
					},
					CanRunManually = null,
				},
				Jobs = new Dictionary<string, Job>
				{
					{
					"CI", new Job
						{
							Name = "CI",
							Uses = "SkylineCommunications/_ReusableWorkflows/.github/workflows/Automation Master Workflow.yml@main",
							With = new Dictionary<string, string>
							{
								{ "referenceName",          "${{ github.ref_name }}" },
								{ "runNumber",              "${{ github.run_number }}" },
								{ "referenceType",          "${{ github.ref_type }}" },
								{ "repository",             "${{ github.repository }}" },
								{ "owner",                  "${{ github.repository_owner }}" },
								{ "sonarCloudProjectName",  sonarcloudProjectName },
							},
							Secrets = new Dictionary<string, string>
							{
								{ "api-key",            "${{ secrets.DATAMINER_DEPLOY_KEY }}" },
								{ "sonarCloudToken",    "${{ secrets.SONAR_TOKEN }}" },
							},
						}
					},
				},
			};

			return flow;
		}

		public static Workflow CreateAutomationCICDWorkflow() => CreateAutomationCICDWorkflow("# Grab your project id from https://sonarcloud.io/project/create.");

		public static Workflow CreateAutomationCICDWorkflow(string sonarcloudProjectName)
		{
			var flow = new Workflow
			{
				Name = "DataMiner CICD Automation",
				On = new On
				{
					Push = new Push
					{
						Branches = new List<string>(),
						Tags = new List<string>
						{
							"[0-9]+.[0-9]+.[0-9]+.[0-9]+",
							"[0-9]+.[0-9]+.[0-9]+-[0-9a-zA-Z]+",
						},
					},
					CanRunManually = null,
				},
				Jobs = new Dictionary<string, Job>
				{
					{
					"CI", new Job
						{
							Name = "CI",
							Uses = "SkylineCommunications/_ReusableWorkflows/.github/workflows/Automation Master Workflow.yml@main",
							With = new Dictionary<string, string>
							{
								{ "referenceName",          "${{ github.ref_name }}" },
								{ "runNumber",              "${{ github.run_number }}" },
								{ "referenceType",          "${{ github.ref_type }}" },
								{ "repository",             "${{ github.repository }}" },
								{ "owner",                  "${{ github.repository_owner }}" },
								{ "sonarCloudProjectName",  sonarcloudProjectName },
							},
							Secrets = new Dictionary<string, string>
							{
								{ "api-key",            "${{ secrets.DATAMINER_DEPLOY_KEY }}" },
								{ "sonarCloudToken",    "${{ secrets.SONAR_TOKEN }}" },
							},
						}
					},
					{
					"CD", new Job
						{
							If = "github.ref_type == 'tag'",
							Environment = "production",
							Name = "CD",
							RunsOn = "ubuntu-latest",
							Needs = "CI",
							Steps = new List<Job>
							{
								new Job
								{
									Uses = "actions/checkout@v3",
								},
								new Job
								{
									Name = "Skyline DataMiner Deploy Action",
									Uses = "SkylineCommunications/Skyline-DataMiner-Deploy-Action@v1",
									With = new Dictionary<string, string>
									{
										{ "stage",          "Deploy" },
										{ "api-key",        "${{ secrets.DATAMINER_DEPLOY_KEY }}" },
										{ "artifact-id",    "${{ needs.CI.outputs.artifact-id }}" },
									},
								},
							},
						}
					},
				},
			};

			return flow;
		}

		public static Workflow CreateNugetCICDWorkflow() => CreateNugetCICDWorkflow("# Grab your project id from https://sonarcloud.io/project/create.");

		public static Workflow CreateNugetCICDWorkflow(string sonarcloudProjectName)
		{
			var flow = new Workflow
			{
				Name = "DataMiner CICD Nuget Solution",
				On = new On
				{
					Push = new Push
					{
						Branches = new List<string>
						{
							"main",
							"master",
						},
						Tags = new List<string>
						{
							"[0-9]+.[0-9]+.[0-9]+",
							"[0-9]+.[0-9]+.[0-9]+-[0-9a-zA-Z]+",
						},
					},
					CanRunManually = null,
				},
				Jobs = new Dictionary<string, Job>
				{
					{
					"CICD", new Job
						{
							Name = "CICD",
							Uses = "SkylineCommunications/_ReusableWorkflows/.github/workflows/NuGet Solution Master Workflow.yml@main",
							With = new Dictionary<string, string>
							{
								{ "referenceName",          "${{ github.ref_name }}" },
								{ "runNumber",              "${{ github.run_number }}" },
								{ "referenceType",          "${{ github.ref_type }}" },
								{ "repository",             "${{ github.repository }}" },
								{ "owner",                  "${{ github.repository_owner }}" },
								{ "sonarCloudProjectName",  sonarcloudProjectName },
							},
							Secrets = new Dictionary<string, string>
							{
								{ "sonarCloudToken",    "${{ secrets.SONAR_TOKEN }}" },
								{ "pfx",                "${{ secrets.PFX }}" },
								{ "pfxPassword",        "${{ secrets.PFXPASSWORD  }}" },
								{ "nugetApiKey",        "${{ secrets.NUGETAPIKEY  }}" },
							},
						}
					},
				},
			};

			return flow;
		}

		public static Workflow CreateConnectorCIWorkflow() => CreateConnectorCIWorkflow("# Grab your project id from https://sonarcloud.io/project/create.");

		public static Workflow CreateConnectorCIWorkflow(string sonarcloudProjectName)
		{
			var flow = new Workflow
			{
				Name = "DataMiner CI Connector",
				On = new On
				{
					Push = new Push
					{
						Branches = new List<string>(),
						Tags = new List<string>
						{
							"[0-9]+.[0-9]+.[0-9]+.[0-9]+",
							"[0-9]+.[0-9]+.[0-9]+-[0-9a-zA-Z]+",
						},
					},
					CanRunManually = null,
				},
				Jobs = new Dictionary<string, Job>
				{
					{
					"CI", new Job
						{
							Name = "CI",
							Uses = "SkylineCommunications/_ReusableWorkflows/.github/workflows/Connector Master Workflow.yml@main",
							With = new Dictionary<string, string>
							{
								{ "referenceName",          "${{ github.ref_name }}" },
								{ "runNumber",              "${{ github.run_number }}" },
								{ "referenceType",          "${{ github.ref_type }}" },
								{ "repository",             "${{ github.repository }}" },
								{ "owner",                  "${{ github.repository_owner }}" },
								{ "sonarCloudProjectName",  sonarcloudProjectName },
							},
							Secrets = new Dictionary<string, string>
							{
								{ "api-key",            "${{ secrets.DATAMINER_DEPLOY_KEY }}" },
								{ "sonarCloudToken",    "${{ secrets.SONAR_TOKEN }}" },
							},
						}
					},
				},
			};

			return flow;
		}

		public static Workflow CreateInternalNugetCICDWorkflow() => CreateInternalNugetCICDWorkflow("# Grab your project id from https://sonarcloud.io/project/create.");

		public static Workflow CreateInternalNugetCICDWorkflow(string sonarcloudProjectName)
		{
			var flow = new Workflow
			{
				Name = "DataMiner CICD Internal Nuget Solution",
				On = new On
				{
					Push = new Push
					{
						Branches = new List<string>
						{
							"main",
							"master",
						},
						Tags = new List<string>
						{
							"[0-9]+.[0-9]+.[0-9]+",
							"[0-9]+.[0-9]+.[0-9]+-[0-9a-zA-Z]+",
						},
					},
					CanRunManually = null,
				},
				Jobs = new Dictionary<string, Job>
				{
					{
					"CICD", new Job
						{
							Name = "CICD",
							Uses = "SkylineCommunications/_ReusableWorkflows/.github/workflows/Internal NuGet Solution Master Workflow.yml@main",
							With = new Dictionary<string, string>
							{
								{ "referenceName",          "${{ github.ref_name }}" },
								{ "runNumber",              "${{ github.run_number }}" },
								{ "referenceType",          "${{ github.ref_type }}" },
								{ "repository",             "${{ github.repository }}" },
								{ "owner",                  "${{ github.repository_owner }}" },
								{ "sonarCloudProjectName",  sonarcloudProjectName },
							},
							Secrets = new Dictionary<string, string>
							{
								{ "sonarCloudToken",    "${{ secrets.SONAR_TOKEN }}" },
								{ "nugetApiKey",        "${{ secrets.NUGETAPIKEY_GITHUB  }}" },
							},
						}
					},
				},
			};

			return flow;
		}
	}
}
