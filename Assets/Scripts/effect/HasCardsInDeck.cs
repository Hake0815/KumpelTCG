using gamecore.card;

namespace gamecore.effect
{
    public class HasCardsInDeck : IPlayCondition
    {
        public bool IsMet(ICard card) => card.Owner.Deck.GetCardCount() > 0;
    }
}
