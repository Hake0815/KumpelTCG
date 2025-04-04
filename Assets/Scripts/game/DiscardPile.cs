using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;

namespace gamecore.game
{
    public interface IDiscardPile
    {
        public List<ICard> Cards { get; }
        public int GetCardCount();
        public ICard GetLastCard();
        public event Action CardsChanged;
    }

    internal interface IDiscardPileLogic : IDiscardPile
    {
        internal void AddCards(List<ICard> cards);
        internal void RemoveCards(List<ICard> cards);
    }

    public class DiscardPile : IDiscardPileLogic
    {
        public List<ICard> Cards { get; } = new();
        public event Action CardsChanged;

        public int GetCardCount()
        {
            return Cards.Count;
        }

        void IDiscardPileLogic.AddCards(List<ICard> cards)
        {
            Cards.AddRange(cards);
            OnCardsChanged();
        }

        void IDiscardPileLogic.RemoveCards(List<ICard> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardsChanged();
        }

        protected virtual void OnCardsChanged()
        {
            CardsChanged?.Invoke();
        }

        public ICard GetLastCard()
        {
            return Cards.LastOrDefault();
        }
    }
}
