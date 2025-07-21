using System.Collections.Generic;
using gamecore.card;

namespace gamecore.instruction.filter
{
    class TrueNode : FilterNode
    {
        public override bool Matches(ICardLogic card, ICardLogic sourceCard) => true;

        public override object ToSerializable()
        {
            return new Dictionary<string, object> { { "op", "true" } };
        }
    }
}
