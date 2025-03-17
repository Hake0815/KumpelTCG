using System.Collections.Generic;

namespace gamecore.card
{
    public interface IDeck
    {
        public void SetUp(List<ICard> cards);
        public ICard Draw();
        public int GetCardCount();
    }

    public class Deck : IDeck
    {
        private List<ICard> Cards { get; set; }

        public void SetUp(List<ICard> cards)
        {
            Cards = cards;
        }

        public ICard Draw()
        {
            if (Cards?.Count > 0)
            {
                var drawnCard = Cards[0];
                Cards.RemoveAt(0);
                return drawnCard;
            }
            return null;
        }

        public int GetCardCount()
        {
            return Cards.Count;
        }
    }
}
