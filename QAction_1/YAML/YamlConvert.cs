// Ignore Spelling: YAML Deserialize

namespace Skyline.Protocol.YAML
{
    using YamlDotNet.Serialization;

    public static class YamlConvert
    {
        private static readonly ISerializer _serializer = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .Build();

        private static readonly IDeserializer _deserializer = new DeserializerBuilder()
            .Build();

        public static string SerializeObject<T>(T obj)
        {
            return _serializer.Serialize(obj);
        }

        public static T DeserializeObject<T>(string obj)
        {
            return _deserializer.Deserialize<T>(obj);
        }

        public static bool IsValidYaml(string obj)
        {
			try
			{
				_deserializer.Deserialize<object>(obj);
				return true;
			}
			catch
			{
				return false;
			}
		}
    }
}
