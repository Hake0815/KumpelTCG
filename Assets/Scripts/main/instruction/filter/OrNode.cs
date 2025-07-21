using System.Collections.Generic;
using System.Linq;
using gamecore.card;

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

        public override object ToSerializable()
        {
            return new Dictionary<string, object>
            {
                { "op", "or" },
                { "operands", Operands.ConvertAll(o => o.ToSerializable()) },
            };
        }
    }
}
