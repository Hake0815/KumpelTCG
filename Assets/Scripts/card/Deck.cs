using System;
using System.Collections.Generic;
using System.Linq;

namespace gamecore.card
{
    public interface IDeck
    {
        public void SetUp(List<ICard> cards);
        public List<ICard> Draw(int amount);
        public void Shuffle();
        public int GetCardCount();
        public event Action CardCountChanged;
    }

    public class Deck : IDeck
    {
        private List<ICard> Cards { get; set; }
        private static Random rng = new();

        public event Action CardCountChanged;

        public void SetUp(List<ICard> cards)
        {
            Cards = cards;
        }

        public List<ICard> Draw(int amount)
        {
            var drawnCards = Cards.GetRange(0, Math.Min(amount, Cards.Count));
            Cards.RemoveRange(0, drawnCards.Count);
            OnCardCountChanged();
            return drawnCards;
        }

        public void Shuffle()
        {
            var n = GetCardCount();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (Cards[n], Cards[k]) = (Cards[k], Cards[n]);
            }
        }

        public int GetCardCount()
        {
            return Cards.Count;
        }

        protected virtual void OnCardCountChanged()
        {
            CardCountChanged?.Invoke();
        }
    }
}
