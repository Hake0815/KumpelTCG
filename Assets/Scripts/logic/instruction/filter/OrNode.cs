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
            var protoBufFilter = new ProtoBufFilter
            {
                LogicalOperator = ProtoBufFilterLogicalOperator.FilterLogicalOperatorOr,
            };
            protoBufFilter.Operands.Capacity = Operands.Count;
            foreach (var operand in Operands)
            {
                protoBufFilter.Operands.Add(operand.ToSerializable());
            }
            return protoBufFilter;
        }
    }
}
