using System.Collections.Generic;

namespace gamecore.instruction
{
    [System.Serializable]
    public class InstructionJson
    {
        public string instruction_type;
        public Dictionary<string, object> args;

        public InstructionJson() { }

        public InstructionJson(string instructionType, Dictionary<string, object> arguments)
        {
            instruction_type = instructionType;
            args = arguments;
        }
    }
}
