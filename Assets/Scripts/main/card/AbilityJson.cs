using System;
using System.Collections.Generic;
using gamecore.common;
using gamecore.instruction;

namespace gamecore.card
{
    [Serializable]
    public class AbilityJson : JsonStringSerializable
    {
        public string Name { get; }
        public List<InstructionJson> Instructions { get; }
        public List<ConditionJson> Conditions { get; }

        public AbilityJson(
            string name,
            List<InstructionJson> instructions = null,
            List<ConditionJson> conditions = null
        )
        {
            Name = name;
            Instructions = instructions ?? new List<InstructionJson>();
            Conditions = conditions ?? new List<ConditionJson>();
        }
    }
}
