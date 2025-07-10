using System.Collections.Generic;
using System.Linq;
using gamecore.card;

namespace gamecore.instruction.filter
{
    class AndNode : FilterNode
    {
        public List<FilterNode> Operands { get; }

        public AndNode(List<FilterNode> operands)
        {
            Operands = operands;
        }

        public override bool Matches(ICardLogic card, ICardLogic sourceCard) =>
            Operands.All(o => o.Matches(card, sourceCard));
    }
}
