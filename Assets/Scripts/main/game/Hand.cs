using System;
using System.Collections.Generic;
using System.Linq;

namespace gamecore.card
{
    public interface IHand
    {
        List<ICard> Cards { get; }
        int CardCount { get; }
        event EventHandler<List<ICard>> CardsAdded;
        event Action CardsRemoved;
    }

    internal interface IHandLogic : IHand
    {
        new List<ICardLogic> Cards { get; }
        void AddCards(List<ICardLogic> cards);
        void RemoveCards(List<ICard> cards);
        void RemoveCard(ICard card);
        void Clear();
    }

    internal class Hand : IHandLogic
    {
        public List<ICardLogic> Cards { get; } = new();
        public event EventHandler<List<ICard>> CardsAdded;
        public event Action CardsRemoved;

        public int CardCount
        {
            get => Cards.Count;
        }

        List<ICard> IHand.Cards => Cards.Cast<ICard>().ToList();

        public void AddCards(List<ICardLogic> cards)
        {
            Cards.AddRange(cards);
            OnCardsAddedToHand(cards.Cast<ICard>().ToList());
        }

        public void RemoveCards(List<ICard> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardsRemoved(cards);
        }

        public void RemoveCard(ICard card)
        {
            Cards.Remove((ICardLogic)card);
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

        public void Clear()
        {
            var removedCards = ((IHand)this).Cards;
            Cards.Clear();
            OnCardsRemoved(removedCards);
        }
    }
}
