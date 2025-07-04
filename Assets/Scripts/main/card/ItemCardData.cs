using System.Collections.Generic;
using gamecore.effect;

namespace gamecore.card
{
    class ItemCardData : TrainerCardData
    {
        public ItemCardData(
            string name,
            string id,
            List<IEffect> effects,
            List<IUseCondition> conditions
        )
            : base(name, id, effects, conditions) { }
    }
}
