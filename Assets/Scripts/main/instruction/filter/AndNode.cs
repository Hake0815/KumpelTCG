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

        public override object ToSerializable()
        {
            return new Dictionary<string, object>
            {
                { "op", "and" },
                { "operands", Operands.ConvertAll(o => o.ToSerializable()) },
            };
        }
    }
}
