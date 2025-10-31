using System;
using System.Collections.Generic;

namespace gamecore.instruction
{
    [Serializable]
    public class InstructionJson
    {
        public InstructionType InstructionType { get; }
        public Dictionary<string, object> Data { get; }

        public InstructionJson(InstructionType instructionType)
            : this(instructionType, new()) { }

        public InstructionJson(InstructionType instructionType, Dictionary<string, object> data)
        {
            InstructionType = instructionType;
            Data = data;
        }
    }
}
