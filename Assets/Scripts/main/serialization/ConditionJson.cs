using System.Collections.Generic;

namespace gamecore.serialization
{
    [System.Serializable]
    public class ConditionJson : IJsonStringSerializable
    {
        public ConditionType ConditionType { get; }
        public List<InstructionDataJson> Data { get; }

        public ConditionJson(ConditionType conditionType, List<InstructionDataJson> data)
        {
            ConditionType = conditionType;
            Data = data;
        }
    }
}
