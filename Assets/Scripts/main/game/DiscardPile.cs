using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;

namespace gamecore.game
{
    public interface IDiscardPile : ICardList
    {
        ICard LastCard { get; }
    }

    internal interface IDiscardPileLogic : IDiscardPile, ICardListLogic { }

    class DiscardPile : IDiscardPileLogic
    {
        public List<ICardLogic> Cards { get; } = new();
        public event Action<List<ICard>> CardCountChanged;

        public int CardCount
        {
            get => Cards.Count;
        }

        public ICard LastCard
        {
            get => Cards.LastOrDefault();
        }

        public void OnCardCountChanged()
        {
            CardCountChanged?.Invoke(((ICardList)this).Cards);
        }
    }
}
