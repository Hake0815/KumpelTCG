using System.Collections.Generic;
using gamecore.instruction;

namespace gamecore.card
{
    class ItemCardData : TrainerCardData
    {
        public ItemCardData(
            string name,
            string id,
            List<IInstruction> instructions,
            List<IUseCondition> conditions
        )
            : base(name, id, instructions, conditions) { }

        protected override CardSubtype GetCardSubtype()
        {
            return CardSubtype.Item;
        }
    }
}
