using System.Collections.Generic;
using gamecore.card;

namespace gamecore.instruction.filter
{
    class ExcludeSourceCardNode : FilterNode
    {
        public override bool Matches(ICardLogic card, ICardLogic sourceCard)
        {
            return card != sourceCard;
        }

        public override object ToSerializable()
        {
            return new Dictionary<string, object> { { "op", "exclude_source" } };
        }
    }
}
