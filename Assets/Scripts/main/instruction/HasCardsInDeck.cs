using gamecore.card;

namespace gamecore.instruction
{
    class HasCardsInDeck : IUseCondition
    {
        public bool IsMet(ICardLogic card) => card.Owner.Deck.CardCount > 0;
    }
}
