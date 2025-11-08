using System.Collections.Generic;

namespace gamecore.serialization
{
    [System.Serializable]
    public class ConditionJson : IJsonStringSerializable
    {
        public ConditionType ConditionType { get; }
        public List<IInstructionDataJson> Data { get; }

        public ConditionJson(ConditionType conditionType, List<IInstructionDataJson> data)
        {
            ConditionType = conditionType;
            Data = data;
        }
    }
}
