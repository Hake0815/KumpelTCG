using System;
using System.Collections.Generic;
using System.Linq;

namespace gamecore.card
{
    public interface IPrizes : ICardList
    {
        event Action<List<ICard>> PrizesTaken;
    }

    internal interface IPrizesLogic : IPrizes, ICardListLogic
    {
        List<ICardLogic> TakePrizes(int amount);
    }

    internal class Prizes : IPrizesLogic
    {
        public List<ICardLogic> Cards { get; } = new();

        public event Action CardCountChanged;
        public event Action<List<ICard>> PrizesTaken;

        public void OnCardCountChanged()
        {
            CardCountChanged?.Invoke();
        }

        public List<ICardLogic> TakePrizes(int amount)
        {
            var prizes = Cards.GetRange(0, Math.Min(amount, Cards.Count));
            Cards.RemoveRange(0, prizes.Count);
            OnCardCountChanged();
            PrizesTaken?.Invoke(prizes.Cast<ICard>().ToList());
            return prizes;
        }
    }
}
