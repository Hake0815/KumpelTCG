using System;
using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game
{
    public interface IDeck
    {
        int GetCardCount();
        event Action CardCountChanged;
    }

    internal interface IDeckLogic : IDeck
    {
        void SetUp(List<ICard> cards);
        List<ICard> Draw(int amount);
        void Shuffle();
        void AddCards(List<ICard> cards);
    }

    internal class Deck : IDeckLogic
    {
        private List<ICard> Cards { get; set; }
        private static readonly Random rng = new();

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

        public void AddCards(List<ICard> cards)
        {
            Cards.AddRange(cards);
            OnCardCountChanged();
        }

        protected virtual void OnCardCountChanged()
        {
            CardCountChanged?.Invoke();
        }
    }
}
