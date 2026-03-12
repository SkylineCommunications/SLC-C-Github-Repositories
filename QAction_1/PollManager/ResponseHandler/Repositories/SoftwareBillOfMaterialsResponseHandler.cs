namespace Skyline.Protocol.PollManager.ResponseHandler.Repositories
{
	using System;
	using System.Linq;
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

			var table = SoftwareBillOfMaterialsTable.GetTable();
			var row = table.Rows.Find(r => r.Name == response.Sbom.Name) ?? new SoftwareBillOfMaterialsTableRow();
			row.RepositoryID = $"{owner}/{repo}";
			row.SpdxId = response.Sbom.SpdxId;
			row.SpdxVersion = response.Sbom.SpdxVersion;
			row.DataLicense = response.Sbom.DataLicense;
			row.DocumentNamespace = response.Sbom.DocumentNamespace;
			row.CreateAt = DateTime.SpecifyKind(response.Sbom.CreationInfo.Created, DateTimeKind.Utc);
			row.LastPolledAt = utcNow;

			// If its a new row fill in Name and add it to the table.
			if (row.Name == Exceptions.NotAvailable)
			{
				row.Name = response.Sbom.Name;
				table.Rows.Add(row);
			}

			table.SaveToProtocol(protocol);

			HandleSoftwareBillOfMaterialsPackagesResponse(protocol, row.RepositoryID, response);
			HandleSoftwareBillOfMaterialsRelationshipsResponse(protocol, row.RepositoryID, response);
		}

		private static void HandleSoftwareBillOfMaterialsPackagesResponse(SLProtocol protocol, string repositoryId, SoftwareBillOfMaterialsResponse response)
		{
			var utcNow = DateTime.UtcNow;
			var table = SoftwareBillOfMaterialsPackagesTable.GetTable();
			foreach (var package in response.Sbom.Packages)
			{
				var id = $"{response.Sbom.Name}/{package.SpdxId}";
				var row = table.Rows.Find(r => r.Instance == id) ?? new SoftwareBillOfMaterialsPackagesTableRow();
				row.SoftwareBillOfMaterialsName = response.Sbom.Name;
				row.RepositoryID = repositoryId;
				row.SpdxId = package.SpdxId;
				row.Name = package.Name;
				row.Version = package.VersionInfo;
				row.DownloadLocation = package.DownloadLocation;
				row.FilesAnalyzed = package.FilesAnalyzed;
				row.LicenseConcluded = package.LicenseConcluded ?? Exceptions.NotAvailable;
				row.LicenseDeclared = package.LicenseDeclared ?? Exceptions.NotAvailable;
				row.Supplier = package.Supplier ?? Exceptions.NotAvailable;
				row.CopyrightText = package.CopyrightText ?? Exceptions.NotAvailable;
				row.LastPolledAt = utcNow;

				if (row.Instance == Exceptions.NotAvailable)
				{
					row.Instance = id;
					table.Rows.Add(row);
				}
			}

			table.SaveToProtocol(protocol, true);
		}

		private static void HandleSoftwareBillOfMaterialsRelationshipsResponse(SLProtocol protocol, string repositoryId, SoftwareBillOfMaterialsResponse response)
		{
			var utcNow = DateTime.UtcNow;
			var table = SoftwareBillOfMaterialsRelationshipsTable.GetTable();
			foreach (var relationship in response.Sbom.Relationships)
			{
				var id = $"{response.Sbom.Name}/{relationship.SpdxElementId}/{relationship.RelatedSpdxElement}";
				var row = table.Rows.Find(r => r.Instance == id) ?? new SoftwareBillOfMaterialsRelationshipsTableRow();
				row.SoftwareBillOfMaterialsName = response.Sbom.Name;
				row.RepositoryID = repositoryId;
				row.SpdxElementID = relationship.SpdxElementId;
				row.RelatedSpdxElementID = relationship.RelatedSpdxElement;
				row.RelationshipType = relationship.RelationshipType;
				row.LastPolledAt = utcNow;

				if (row.Instance == Exceptions.NotAvailable)
				{
					row.Instance = id;
					table.Rows.Add(row);
				}
			}

			table.SaveToProtocol(protocol, true);
		}
	}
}
