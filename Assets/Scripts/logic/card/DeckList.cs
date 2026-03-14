using System.Collections.Generic;
using System.Linq;

namespace gamecore.card
{
    interface IDeckList
    {
        List<ICardLogic> Cards { get; }
        List<ICardLogic> GetCardsByDeckIds(List<int> deckIds);
        List<ICardLogic> GetCardsByDeckIds(List<ICardLogic> cardStubs);
        ICardLogic GetCardByDeckId(int deckId);
    }

    class DeckList : IDeckList
    {
        public List<ICardLogic> Cards { get; }

        public DeckList(List<ICardLogic> cards) => Cards = cards;

        public List<ICardLogic> GetCardsByDeckIds(List<int> deckIds)
        {
            var resultCards = new List<ICardLogic>();
            foreach (var card in Cards)
            {
                if (deckIds.Contains(card.DeckId))
                {
                    resultCards.Add(card);
                }
            }
            return resultCards;
        }

        public List<ICardLogic> GetCardsByDeckIds(List<ICardLogic> cardStubs)
        {
            return GetCardsByDeckIds(cardStubs.Select(card => card.DeckId).ToList());
        }

        public ICardLogic GetCardByDeckId(int deckId)
        {
            foreach (var card in Cards)
            {
                if (card.DeckId == deckId)
                {
                    return card;
                }
            }
            return null;
        }
    }
}
