using gamecore.card;

namespace gamecore.effect
{
    internal class HasCardsInDeck : IPlayCondition
    {
        public bool IsMet(ICardLogic card) => card.Owner.Deck.CardCount > 0;
    }
}
