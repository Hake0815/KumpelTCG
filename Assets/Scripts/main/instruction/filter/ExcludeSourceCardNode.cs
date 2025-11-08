using System.Collections.Generic;
using gamecore.card;
using gamecore.serialization;

namespace gamecore.instruction.filter
{
    class ExcludeSourceCardNode : FilterNode
    {
        public override bool Matches(ICardLogic card, ICardLogic sourceCard)
        {
            return card != sourceCard;
        }

        public override FilterJson ToSerializable()
        {
            return new FilterJson(leafType: LeafType.ExcludeSource);
        }
    }
}
