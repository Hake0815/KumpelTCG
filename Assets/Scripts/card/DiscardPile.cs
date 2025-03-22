using System;
using System.Collections.Generic;
using System.Linq;

namespace gamecore.card
{
    public interface IDiscardPile
    {
        public List<ICard> Cards { get; }
        public int GetCardCount();
        public void AddCards(List<ICard> cards);
        public void RemoveCards(List<ICard> cards);
        public ICard GetLastCard();
        public event Action CardsChanged;
    }

    public class DiscardPile : IDiscardPile
    {
        public List<ICard> Cards { get; } = new();
        public event Action CardsChanged;

        public int GetCardCount()
        {
            return Cards.Count;
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

        public ICard GetLastCard()
        {
            return Cards.LastOrDefault();
        }
    }
}
