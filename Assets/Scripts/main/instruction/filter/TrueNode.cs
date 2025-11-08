using System.Collections.Generic;
using gamecore.card;
using gamecore.serialization;

namespace gamecore.instruction.filter
{
    class TrueNode : FilterNode
    {
        public override bool Matches(ICardLogic card, ICardLogic sourceCard) => true;

        public override FilterJson ToSerializable()
        {
            return new FilterJson(leafType: LeafType.True);
        }
    }
}
