using gamecore.card;

namespace gamecore.instruction.filter
{
    class ExcludeSourceCardNode : FilterNode
    {
        public override bool Matches(ICardLogic card, ICardLogic sourceCard)
        {
            return card != sourceCard;
        }
    }
}
