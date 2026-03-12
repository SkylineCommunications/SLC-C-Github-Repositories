namespace Skyline.Protocol.API.Dependency
{
	using System;
	using System.Collections.Generic;

	using Newtonsoft.Json;

	public class SoftwareBillOfMaterialsResponse
	{
		[JsonProperty("sbom", Required = Required.Always)]
		public Sbom Sbom { get; set; }
	}

	public class Sbom
	{
		[JsonProperty("SPDXID", Required = Required.Always)]
		public string SpdxId { get; set; }

		[JsonProperty("spdxVersion", Required = Required.Always)]
		public string SpdxVersion { get; set; }

		[JsonProperty("comment")]
		public string Comment { get; set; }

		[JsonProperty("creationInfo", Required = Required.Always)]
		public CreationInfo CreationInfo { get; set; }

		[JsonProperty("name", Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty("dataLicense", Required = Required.Always)]
		public string DataLicense { get; set; }

		[JsonProperty("documentNamespace", Required = Required.Always)]
		public string DocumentNamespace { get; set; }

		[JsonProperty("packages", Required = Required.Always)]
		public List<SpdxPackage> Packages { get; set; }

		[JsonProperty("relationships")]
		public List<Relationship> Relationships { get; set; }
	}

	public class CreationInfo
	{
		[JsonProperty("created", Required = Required.Always)]
		public DateTime Created { get; set; }

		[JsonProperty("creators", Required = Required.Always)]
		public List<string> Creators { get; set; }
	}

	public class SpdxPackage
	{
		[JsonProperty("SPDXID", Required = Required.Always)]
		public string SpdxId { get; set; }

		[JsonProperty("name", Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty("versionInfo", Required = Required.Always)]
		public string VersionInfo { get; set; }

		[JsonProperty("downloadLocation", Required = Required.Always)]
		public string DownloadLocation { get; set; }

		[JsonProperty("filesAnalyzed", Required = Required.Always)]
		public bool FilesAnalyzed { get; set; }

		[JsonProperty("licenseConcluded")]
		public string LicenseConcluded { get; set; }

		[JsonProperty("licenseDeclared")]
		public string LicenseDeclared { get; set; }

		[JsonProperty("supplier")]
		public string Supplier { get; set; }

		[JsonProperty("copyrightText")]
		public string CopyrightText { get; set; }

		[JsonProperty("externalRefs")]
		public List<ExternalReference> ExternalRefs { get; set; }
	}

	public class ExternalReference
	{
		[JsonProperty("referenceCategory", Required = Required.Always)]
		public string ReferenceCategory { get; set; }

		[JsonProperty("referenceLocator", Required = Required.Always)]
		public string ReferenceLocator { get; set; }

		[JsonProperty("referenceType", Required = Required.Always)]
		public string ReferenceType { get; set; }
	}

	public class Relationship
	{
		[JsonProperty("relationshipType", Required = Required.Always)]
		public string RelationshipType { get; set; }

		[JsonProperty("spdxElementId", Required = Required.Always)]
		public string SpdxElementId { get; set; }

		[JsonProperty("relatedSpdxElement", Required = Required.Always)]
		public string RelatedSpdxElement { get; set; }
	}
}
