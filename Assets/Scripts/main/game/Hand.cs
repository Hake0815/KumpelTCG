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
        public event Action CardCountChanged;

        public void OnCardCountChanged()
        {
            CardCountChanged?.Invoke();
        }
    }
}
