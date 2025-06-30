using gamecore.card;

namespace gamecore.effect
{
    class HasAtLeatCardsInHand : IUseCondition
    {
        public int Count { get; }

        public HasAtLeatCardsInHand(int count)
        {
            Count = count;
        }

        public bool IsMet(ICardLogic card)
        {
            return card.Owner.Hand.CardCount >= Count;
        }
    }
}
