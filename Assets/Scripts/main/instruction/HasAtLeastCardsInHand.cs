using gamecore.card;

namespace gamecore.instruction
{
    class HasAtLeastCardsInHand : IUseCondition
    {
        public int Count { get; }

        public HasAtLeastCardsInHand(int count)
        {
            Count = count;
        }

        public bool IsMet(ICardLogic card)
        {
            return card.Owner.Hand.CardCount >= Count;
        }
    }
}
