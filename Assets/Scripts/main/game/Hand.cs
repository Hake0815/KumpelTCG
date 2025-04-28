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

    internal class Hand : IHandLogic
    {
        public List<ICardLogic> Cards { get; } = new();

        public event Action CardCountChanged;

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public void OnCardCountChanged()
        {
            CardCountChanged?.Invoke();
        }
    }
}
