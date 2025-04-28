using System;
using System.Collections.Generic;

namespace gamecore.card
{
    public interface IPrizes : ICardList { }

    internal interface IPrizesLogic : IPrizes, ICardListLogic { }

    internal class Prizes : IPrizesLogic
    {
        public List<ICardLogic> Cards { get; } = new();

        public event Action CardCountChanged;

        public void OnCardCountChanged()
        {
            CardCountChanged?.Invoke();
        }
    }
}
