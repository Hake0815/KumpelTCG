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
            var amountToTake = Math.Min(amount, Cards.Count);
            var prizes = Cards.GetRange(Cards.Count - amountToTake, amountToTake);
            Cards.RemoveRange(Cards.Count - amountToTake, amountToTake);
            OnCardCountChanged();
            PrizesTaken?.Invoke(prizes.Cast<ICard>().ToList());
            return prizes;
        }
    }
}
