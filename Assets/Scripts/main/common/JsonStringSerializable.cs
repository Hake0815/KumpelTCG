using Newtonsoft.Json;

namespace gamecore.common
{
    public abstract class JsonStringSerializable
    {
        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
