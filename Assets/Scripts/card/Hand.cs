using System;
using System.Collections.Generic;

namespace gamecore.card
{
    public interface IHand
    {
        public List<ICard> Cards { get; }
        public int GetCardCount();
        public void AddCards(List<ICard> cards);
        public void RemoveCards(List<ICard> cards);
        public event EventHandler<List<ICard>> CardsAdded;
        public event Action CardsRemoved;
    }

    public class Hand : IHand
    {
        public List<ICard> Cards { get; } = new();
        public event EventHandler<List<ICard>> CardsAdded;
        public event Action CardsRemoved;

        public int GetCardCount()
        {
            return Cards.Count;
        }

        public void AddCards(List<ICard> cards)
        {
            Cards.AddRange(cards);
            OnCardsAddedToHand(cards);
        }

        public void RemoveCards(List<ICard> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardsRemoved(cards);
        }

        protected virtual void OnCardsAddedToHand(List<ICard> cards)
        {
            CardsAdded?.Invoke(this, cards);
        }

        protected virtual void OnCardsRemoved(List<ICard> cards)
        {
            CardsRemoved?.Invoke();
        }
    }
}
