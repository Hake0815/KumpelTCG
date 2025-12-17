using System;
using System.Collections.Generic;
using gamecore.instruction;

namespace gamecore.serialization
{
    [Serializable]
    public class AbilityJson : IJsonStringSerializable
    {
        public string Name { get; }
        public List<InstructionJson> Instructions { get; }
        public List<ConditionJson> Conditions { get; }

        public AbilityJson(
            string name,
            List<InstructionJson> instructions,
            List<ConditionJson> conditions
        )
        {
            Name = name;
            Instructions = instructions;
            Conditions = conditions;
        }
    }
}
