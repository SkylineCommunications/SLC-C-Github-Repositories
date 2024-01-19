namespace Skyline.DataMiner.Utils.Github.API.V20221128.Organizations
{
    using System;

    using Newtonsoft.Json;

    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Organization>>(myJsonResponse);
    public class Organization
    {
        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("node_id")]
        public string NodeId { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; } 

        [JsonProperty("repos_url")]
        public Uri ReposUrl { get; set; }

        [JsonProperty("events_url")]
        public Uri EventsUrl { get; set; }

        [JsonProperty("hooks_url")]
        public Uri HooksUrl { get; set; }

        [JsonProperty("issues_url")]
        public Uri IssuesUrl { get; set; }

        [JsonProperty("members_url")]
        public Uri MembersUrl { get; set; }

        [JsonProperty("public_members_url")]
        public Uri PublicMembersUrl { get; set; }

        [JsonProperty("avatar_url")]
        public Uri AvatarUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
