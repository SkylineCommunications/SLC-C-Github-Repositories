namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;

	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;
	using Skyline.Protocol.API.Dependency;
	using Skyline.Protocol.Extensions;
	using Skyline.Protocol.Tables;

	public static partial class RepositoriesResponseHandler
	{
		public static void HandleSoftwareBillOfMaterialsResponse(SLProtocol protocol)
		{
			// Check status code
			if (!protocol.IsSuccessStatusCode())
			{
				return;
			}

			// Parse response
			var response = SecureNewtonsoftDeserialization.DeserializeObject<SoftwareBillOfMaterialsResponse>(
				Convert.ToString(protocol.GetParameter(Parameter.getrepositorysoftwarebillofmaterialscontent_206)));
			if (response == null)
			{
				protocol.Log($"QA{protocol.QActionID}|{nameof(HandleSoftwareBillOfMaterialsResponse)}|response was null.", LogType.Error, LogLevel.Level1);
				return;
			}

			// Parse name to check which repository this SBOM is linked to
			var pattern = @"com\.github\.(?<owner>.*)\/(?<repo>.*)";
			var options = RegexOptions.Multiline;

			var utcNow = DateTime.UtcNow;
			var match = Regex.Match(response.Sbom.Name, pattern, options);
			var owner = match.Groups["owner"].Value;
			var repo = match.Groups["repo"].Value;

			var row = new SoftwareBillOfMaterialsModel
			{
				Name = response.Sbom.Name,
				RepositoryID = $"{owner}/{repo}",
				SpdxId = response.Sbom.SpdxId,
				SpdxVersion = response.Sbom.SpdxVersion,
				DataLicense = response.Sbom.DataLicense,
				DocumentNamespace = response.Sbom.DocumentNamespace,
				CreatedAt = DateTime.SpecifyKind(response.Sbom.CreationInfo.Created, DateTimeKind.Utc),
				LastPolledAt = utcNow,
			};

			if (SLTables.SoftwareBillOfMaterials.Exists(protocol, row.Name))
			{
				SLTables.SoftwareBillOfMaterials.SetRow(protocol, SoftwareBillOfMaterialsRowConverter.Instance.ToRawValue(row));
			}
			else
			{
				SLTables.SoftwareBillOfMaterials.AddRow(protocol, SoftwareBillOfMaterialsRowConverter.Instance.ToRawValue(row));
			}

			HandleSoftwareBillOfMaterialsPackagesResponse(protocol, row.RepositoryID, response);
			HandleSoftwareBillOfMaterialsRelationshipsResponse(protocol, row.RepositoryID, response);
		}

		private static void HandleSoftwareBillOfMaterialsPackagesResponse(SLProtocol protocol, string repositoryId, SoftwareBillOfMaterialsResponse response)
		{
			var utcNow = DateTime.UtcNow;
			var rows = new List<RepositorysoftwarebillofmaterialspackagesQActionRow>();
			foreach (var package in response.Sbom.Packages)
			{
				var id = $"{response.Sbom.Name}/{package.SpdxId}";
				var row = new SoftwareBillOfMaterialsPackagesModel
				{
					Instance = id,
					SoftwareBillOfMaterialsName = response.Sbom.Name,
					RepositoryID = repositoryId,
					SpdxId = package.SpdxId,
					Name = package.Name,
					Version = package.VersionInfo,
					DownloadLocation = package.DownloadLocation,
					FilesAnalyzed = package.FilesAnalyzed,
					LicenseConcluded = package.LicenseConcluded ?? Exceptions.NotAvailable,
					LicenseDeclared = package.LicenseDeclared ?? Exceptions.NotAvailable,
					Supplier = package.Supplier ?? Exceptions.NotAvailable,
					CopyrightText = package.CopyrightText ?? Exceptions.NotAvailable,
					LastPolledAt = utcNow,
				};

				rows.Add(SoftwareBillOfMaterialsPackagesRowConverter.Instance.ToRawValue(row));
			}

			SLTables.SoftwareBillOfMaterialsPackages.FillTableNoDelete(protocol, rows);
			SLTables.SoftwareBillOfMaterialsPackages.Cleanup(protocol, repositoryId);
		}

		private static void HandleSoftwareBillOfMaterialsRelationshipsResponse(SLProtocol protocol, string repositoryId, SoftwareBillOfMaterialsResponse response)
		{
			var utcNow = DateTime.UtcNow;
			var rows = new List<RepositorysoftwarebillofmaterialsrelationshipsQActionRow>();
			foreach (var relationship in response.Sbom.Relationships)
			{
				var id = $"{response.Sbom.Name}/{relationship.SpdxElementId}/{relationship.RelatedSpdxElement}";
				var row = new SoftwareBillOfMaterialsRelationshipsModel
				{
					Instance = id,
					SoftwareBillOfMaterialsName = response.Sbom.Name,
					RepositoryID = repositoryId,
					SpdxElementID = relationship.SpdxElementId,
					RelatedSpdxElementID = relationship.RelatedSpdxElement,
					RelationshipType = relationship.RelationshipType,
					LastPolledAt = utcNow,
				};

				rows.Add(SoftwareBillOfMaterialsRelationshipsRowConverter.Instance.ToRawValue(row));
			}

			SLTables.SoftwareBillOfMaterialsRelationships.FillTableNoDelete(protocol, rows);
			SLTables.SoftwareBillOfMaterialsRelationships.Cleanup(protocol, repositoryId);
		}
	}
}
