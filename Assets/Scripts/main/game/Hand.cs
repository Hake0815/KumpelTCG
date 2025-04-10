using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace gamecore.card
{
    internal interface IHandLogic : ICardListLogic { }

    internal class Hand : IHandLogic
    {
        public List<ICardLogic> Cards { get; } = new();
        public event EventHandler<List<ICard>> CardsAdded;
        public event Action CardsRemoved;

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
            var removedCards = ((ICardList)this).Cards;
            Cards.Clear();
            OnCardsRemoved(removedCards);
        }

        public IEnumerator GetEnumerator()
        {
            return Cards.GetEnumerator();
        }
    }
}
