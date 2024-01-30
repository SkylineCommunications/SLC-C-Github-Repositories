// Ignore Spelling: Workflow API Workflows

namespace Skyline.Protocol.API.Workflows
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    using YamlDotNet.Serialization;

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Job
    {
        [JsonProperty("if", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "if")]
        public string If { get; set; }

        [JsonProperty("environment", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "environment")]
        public string Environment { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [JsonProperty("uses", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "uses")]
        public string Uses { get; set; }

        [JsonProperty("runs-on", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "runs-on")]
        public string RunsOn { get; set; }

        [JsonProperty("needs", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "needs")]
        public string Needs { get; set; }

        [JsonProperty("with", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "with")]
        public Dictionary<string, string> With { get; set; }

        [JsonProperty("secrets", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "secrets")]
        public Dictionary<string, string> Secrets { get; set; }

        [JsonProperty("steps", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "steps")]
        public List<Job> Steps { get; set; }
    }

    public class On
    {
        [JsonProperty("push", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "push")]
        public Push Push { get; set; }

        [JsonProperty("workflow_dispatch", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "workflow_dispatch", DefaultValuesHandling = DefaultValuesHandling.Preserve)]
        public string CanRunManually { get; set; }
    }

    public class Push
    {
        [JsonProperty("branches", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "branches")]
        public List<string> Branches { get; set; }

        [JsonProperty("tags", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "tags")]
        public List<string> Tags { get; set; }
    }

    public class Workflow
    {
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "name")]
        public string Name { get; set; }

        [JsonProperty("on", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "on")]
        public On On { get; set; }

        [JsonProperty("jobs", NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(Alias = "jobs")]
        public Dictionary<string, Job> Jobs { get; set; }
    }
}