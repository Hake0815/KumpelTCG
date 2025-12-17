using System.Collections.Generic;
using System.Linq;
using gamecore.card;
using gamecore.serialization;

namespace gamecore.instruction.filter
{
    class OrNode : FilterNode
    {
        public List<FilterNode> Operands { get; }

        public OrNode(List<FilterNode> operands)
        {
            Operands = operands;
        }

        public override bool Matches(ICardLogic card, ICardLogic sourceCard) =>
            Operands.Any(o => o.Matches(card, sourceCard));

        public override ProtoBufFilter ToSerializable()
        {
            return new ProtoBufFilter
            {
                Operands = { Operands.ConvertAll(o => o.ToSerializable()) },
                LogicalOperator = ProtoBufFilterLogicalOperator.FilterLogicalOperatorOr,
            };
        }
    }
}
