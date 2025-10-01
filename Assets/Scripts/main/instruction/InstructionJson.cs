using System;
using System.Collections.Generic;

namespace gamecore.instruction
{
    [Serializable]
    public class InstructionJson
    {
        public string InstructionType { get; }
        public Dictionary<string, object> Data { get; }

        public InstructionJson(string instructionType)
            : this(instructionType, new()) { }

        public InstructionJson(string instructionType, Dictionary<string, object> data)
        {
            InstructionType = instructionType;
            Data = data;
        }
    }
}
