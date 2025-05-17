using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace gamecore.card
{
    public interface IHand : ICardList { }

    internal interface IHandLogic : IHand, ICardListLogic
    {
        new List<ICardLogic> Cards { get; }
    }

    class Hand : IHandLogic
    {
        public List<ICardLogic> Cards { get; } = new();

        public event Action CardCountChanged;

        public void OnCardCountChanged()
        {
            CardCountChanged?.Invoke();
        }
    }
}
