using System;
using System.Collections.Generic;
using System.Linq;

namespace gamecore.card
{
    public interface IDeck
    {
        public void SetUp(List<ICard> cards);
        public List<ICard> Draw(int amount);
        public int GetCardCount();
        public event Action CardCountChanged;
    }

    public class Deck : IDeck
    {
        private List<ICard> Cards { get; set; }

        public event Action CardCountChanged;

        public void SetUp(List<ICard> cards)
        {
            Cards = cards;
        }

        public List<ICard> Draw(int amount)
        {
            var drawnCards = Cards.Take(amount).ToList();
            Cards.RemoveRange(0, drawnCards.Count);
            OnCardCountChanged();
            return drawnCards;
        }

        public int GetCardCount()
        {
            return Cards.Count;
        }

        protected virtual void OnCardCountChanged()
        {
            CardCountChanged?.Invoke();
        }
    }
}
