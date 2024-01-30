// Ignore Spelling: Yaml

namespace Skyline.Protocol.YAML.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class YamlPropertyAttribute : Attribute
    {
        public YamlPropertyAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }
    }
}
