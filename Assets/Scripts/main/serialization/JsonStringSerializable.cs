using Newtonsoft.Json;

namespace gamecore.serialization
{
    public interface IJsonStringSerializable { }

    public static class JsonStringSerializableExtensions
    {
        public static string ToJsonString(this IJsonStringSerializable jsonStringSerializable)
        {
            return JsonConvert.SerializeObject(jsonStringSerializable);
        }
    }
}
