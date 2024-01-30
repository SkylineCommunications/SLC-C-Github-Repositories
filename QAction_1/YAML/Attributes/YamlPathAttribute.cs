// Ignore Spelling: Yaml

namespace Skyline.Protocol.YAML.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class YamlPathAttribute : Attribute
    {
        public YamlPathAttribute(params string[] path)
        {
            this.Path = path;
        }

        public string[] Path { get; set; }
    }
}
