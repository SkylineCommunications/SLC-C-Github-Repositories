namespace Skyline.DataMiner.Utils.Github.API.V20221128.Repositories
{
    using Newtonsoft.Json;

    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class Commit
    {
        [JsonProperty("sha")]
        public string Sha { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class RepositoryTagsResponse
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("zipball_url")]
        public string ZipballUrl { get; set; }

        [JsonProperty("tarball_url")]
        public string TarballUrl { get; set; }

        [JsonProperty("commit")]
        public Commit Commit { get; set; }

        [JsonProperty("node_id")]
        public string NodeId { get; set; }
    }
}
