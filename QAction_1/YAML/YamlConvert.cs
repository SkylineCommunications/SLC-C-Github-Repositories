// Ignore Spelling: YAML Deserialize

namespace Skyline.Protocol.YAML
{
    using YamlDotNet.Serialization;

    public static class YamlConvert
    {
        public static string SerializeObject<T>(T obj)
        {
            var serializer = new SerializerBuilder()
                .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
                .Build();
            return serializer.Serialize(obj);
        }

        public static T DeserializeObject<T>(string obj)
        {
            var deserializer = new DeserializerBuilder()
                .Build();

            var result = deserializer.Deserialize<T>(obj);
            return result;
        }

        public static bool IsValidYaml(string obj)
        {
			try
			{
				var deserializer = new DeserializerBuilder()
					.Build();
				deserializer.Deserialize<object>(obj);
				return true;
			}
			catch
			{
				return false;
			}
		}
    }
}
