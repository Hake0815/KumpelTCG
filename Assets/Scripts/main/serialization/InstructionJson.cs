using System;
using System.Collections.Generic;

namespace gamecore.serialization
{
    [Serializable]
    public class InstructionJson : IJsonStringSerializable
    {
        public InstructionType InstructionType { get; }
        public List<IInstructionDataJson> Data { get; }

        public InstructionJson(InstructionType instructionType)
            : this(instructionType, new()) { }

        public InstructionJson(InstructionType instructionType, List<IInstructionDataJson> data)
        {
            InstructionType = instructionType;
            Data = data;
        }
    }
}
