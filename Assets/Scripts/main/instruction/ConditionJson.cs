using System.Collections.Generic;

namespace gamecore.instruction
{
    [System.Serializable]
    public class ConditionJson
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
