using System;
using System.Collections.Generic;

namespace gamecore.card
{
    public interface IHand
    {
        public List<ICard> Cards { get; }
        public int GetCardCount();
        public event EventHandler<List<ICard>> CardsAdded;
        public event Action CardsRemoved;
    }

    internal interface IHandLogic : IHand
    {
        internal void AddCards(List<ICard> cards);
        internal void RemoveCards(List<ICard> cards);
        internal void RemoveCard(ICard card);
        internal void Clear();
    }

    public class Hand : IHandLogic
    {
        public List<ICard> Cards { get; } = new();
        public event EventHandler<List<ICard>> CardsAdded;
        public event Action CardsRemoved;

        public int GetCardCount()
        {
            return Cards.Count;
        }

        void IHandLogic.AddCards(List<ICard> cards)
        {
            Cards.AddRange(cards);
            OnCardsAddedToHand(cards);
        }

        void IHandLogic.RemoveCards(List<ICard> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardsRemoved(cards);
        }

        void IHandLogic.RemoveCard(ICard card)
        {
            Cards.Remove(card);
            OnCardsRemoved(new() { card });
        }

        protected virtual void OnCardsAddedToHand(List<ICard> cards)
        {
            CardsAdded?.Invoke(this, cards);
        }

        protected virtual void OnCardsRemoved(List<ICard> cards)
        {
            CardsRemoved?.Invoke();
        }

        void IHandLogic.Clear()
        {
            Cards.Clear();
            OnCardsRemoved(Cards);
        }
    }
}
