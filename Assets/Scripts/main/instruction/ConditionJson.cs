using System.Collections.Generic;
using gamecore.common;

namespace gamecore.instruction
{
    [System.Serializable]
    public class ConditionJson : JsonStringSerializable
    {
        public string ConditionType { get; }
        public Dictionary<string, object> Data { get; }

        public ConditionJson(string conditionType, Dictionary<string, object> data)
        {
            ConditionType = conditionType;
            Data = data;
        }
    }
}
