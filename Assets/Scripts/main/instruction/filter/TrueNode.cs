using gamecore.card;

namespace gamecore.instruction.filter
{
    class TrueNode : FilterNode
    {
        public override bool Matches(ICardLogic card, ICardLogic sourceCard) => true;
    }
}
