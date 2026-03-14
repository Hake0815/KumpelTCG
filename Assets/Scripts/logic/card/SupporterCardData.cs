using System.Collections.Generic;
using gamecore.instruction;

namespace gamecore.card
{
    class SupporterCardData : TrainerCardData
    {
        public SupporterCardData(
            string name,
            string id,
            List<IInstruction> instructions,
            List<IUseCondition> conditions
        )
            : base(name, id, instructions, conditions) { }
    }
}
