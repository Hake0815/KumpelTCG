using System;
using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game
{
    public interface IDeck
    {
        int CardCount { get; }
        event Action CardCountChanged;
    }

    internal interface IDeckLogic : IDeck
    {
        List<ICardLogic> Draw(int amount);
        void Shuffle();
        void AddCards(List<ICardLogic> cards);
    }

    internal class Deck : IDeckLogic
    {
        private List<ICardLogic> Cards { get; set; }
        private static readonly Random rng = new();
        public int CardCount
        {
            get => Cards.Count;
        }

        public event Action CardCountChanged;

        public Deck(List<ICardLogic> cards)
        {
            Cards = cards;
        }

        public List<ICardLogic> Draw(int amount)
        {
            var drawnCards = Cards.GetRange(0, Math.Min(amount, Cards.Count));
            Cards.RemoveRange(0, drawnCards.Count);
            OnCardCountChanged();
            return drawnCards;
        }

        public void Shuffle()
        {
            var n = CardCount;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (Cards[n], Cards[k]) = (Cards[k], Cards[n]);
            }
        }

        public void AddCards(List<ICardLogic> cards)
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
