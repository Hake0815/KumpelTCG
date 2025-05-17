using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;

namespace gamecore.game
{
    public interface IDiscardPile
    {
        List<ICard> Cards { get; }
        int CardCount { get; }
        ICard LastCard { get; }
        event Action CardsChanged;
    }

    internal interface IDiscardPileLogic : IDiscardPile
    {
        void AddCards(List<ICard> cards);
        void RemoveCards(List<ICard> cards);
    }

    class DiscardPile : IDiscardPileLogic
    {
        public List<ICard> Cards { get; } = new();
        public event Action CardsChanged;

        public int CardCount
        {
            get => Cards.Count;
        }

        public ICard LastCard
        {
            get => Cards.LastOrDefault();
        }

        public void AddCards(List<ICard> cards)
        {
            Cards.AddRange(cards);
            OnCardsChanged();
        }

        public void RemoveCards(List<ICard> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardsChanged();
        }

        protected virtual void OnCardsChanged()
        {
            CardsChanged?.Invoke();
        }
    }
}
