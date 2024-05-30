// Ignore Spelling: API Utils Github Wiki Gitignore Rebase

namespace Skyline.DataMiner.Utils.Github.API.V20221128.Organizations
{
	using System;

	using Newtonsoft.Json;

	[Serializable]
	public class CreateRepository
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("homepage")]
		public string Homepage { get; set; }

		[JsonProperty("private")]
		public bool Private { get; set; }

		[JsonProperty("visibility")]
		public string Visibility { get; set; }

		[JsonProperty("has_issues")]
		public bool? HasIssues { get; set; }

		[JsonProperty("has_projects")]
		public bool? HasProjects { get; set; }

		[JsonProperty("has_wiki")]
		public bool? HasWiki { get; set; }

		[JsonProperty("has_downloads")]
		public bool? HasDownloads { get; set; }

		[JsonProperty("is_template")]
		public bool? IsTemplate { get; set; }

		[JsonProperty("team_id")]
		public int? TeamId { get; set; }

		[JsonProperty("auto_init")]
		public bool? AutoInit { get; set; }

		[JsonProperty("gitignore_template")]
		public string GitignoreTemplate { get; set; }

		[JsonProperty("license_template")]
		public string LicenseTemplate { get; set; }

		[JsonProperty("allow_squash_merge")]
		public bool? AllowSquashMerge { get; set; }

		[JsonProperty("allow_merge_commit")]
		public bool? AllowMergeCommit { get; set; }

		[JsonProperty("allow_rebase_merge")]
		public bool? AllowRebaseMerge { get; set; }

		[JsonProperty("allow_auto_merge")]
		public bool? AllowAutoMerge { get; set; }

		[JsonProperty("delete_branch_on_merge")]
		public bool DeleteBranchOnMerge { get; set; }

		[JsonProperty("use_squash_pr_title_as_default")]
		public bool? UseSquashPrTitleAsDefault { get; set; }

		[JsonProperty("squash_merge_commit_title")]
		public string SquashMergeCommitTitle { get; set; }

		[JsonProperty("squash_merge_commit_message")]
		public string SquashMergeCommitMessage { get; set; }

		[JsonProperty("merge_commit_title")]
		public string MergeCommitTitle { get; set; }

		[JsonProperty("merge_commit_message")]
		public string MergeCommitMessage { get; set; }

		[JsonProperty("custom_properties")]
		public object CustomProperties { get; set; }
	}
}
