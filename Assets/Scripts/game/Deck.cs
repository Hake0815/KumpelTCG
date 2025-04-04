using System;
using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game
{
    public interface IDeck
    {
        public int GetCardCount();
        public event Action CardCountChanged;
    }

    internal interface IDeckLogic : IDeck
    {
        internal void SetUp(List<ICard> cards);
        internal List<ICard> Draw(int amount);
        internal void Shuffle();
        internal void AddCards(List<ICard> cards);
    }

    internal class Deck : IDeckLogic
    {
        private List<ICard> Cards { get; set; }
        private static readonly Random rng = new();

        public event Action CardCountChanged;

        void IDeckLogic.SetUp(List<ICard> cards)
        {
            Cards = cards;
        }

        List<ICard> IDeckLogic.Draw(int amount)
        {
            var drawnCards = Cards.GetRange(0, Math.Min(amount, Cards.Count));
            Cards.RemoveRange(0, drawnCards.Count);
            OnCardCountChanged();
            return drawnCards;
        }

        void IDeckLogic.Shuffle()
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

        void IDeckLogic.AddCards(List<ICard> cards)
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
