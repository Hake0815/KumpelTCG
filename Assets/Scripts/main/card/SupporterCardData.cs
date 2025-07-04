using System.Collections.Generic;
using gamecore.effect;

namespace gamecore.card
{
    class SupporterCardData : TrainerCardData
    {
        public SupporterCardData(
            string name,
            string id,
            List<IEffect> effects,
            List<IUseCondition> conditions
        )
            : base(name, id, effects, conditions) { }
    }
}
