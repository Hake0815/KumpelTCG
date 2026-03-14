using System;
using System.Collections.Generic;
using System.Linq;

namespace gamecore.card
{
    public interface IPrizes : ICardList
    {
        event Action<List<ICard>> PrizesTaken;
    }

    internal abstract class PrizesLogicAbstract : CardListLogicAbstract, IPrizes
    {
        protected PrizesLogicAbstract()
            : base(new()) { }

        public abstract event Action BecameKnown;
        public abstract event Action<List<ICard>> PrizesTaken;

        public abstract List<ICardLogic> TakePrizes(int amount);

        public abstract void DeckSearched();
        public abstract bool AreKnown();
    }

    class Prizes : PrizesLogicAbstract
    {
        private bool _known = false;

        public override event Action<List<ICard>> CardCountChanged;
        public override event Action<List<ICard>> PrizesTaken;
        public override event Action BecameKnown;

        public Prizes()
            : base() { }

        public override bool AreKnown() => _known;

        public override void OnCardCountChanged()
        {
            CardCountChanged?.Invoke(((ICardList)this).Cards);
        }

        public override List<ICardLogic> TakePrizes(int amount)
        {
            var amountToTake = Math.Min(amount, Cards.Count);
            var prizes = Cards.GetRange(Cards.Count - amountToTake, amountToTake);
            Cards.RemoveRange(Cards.Count - amountToTake, amountToTake);
            OnCardCountChanged();
            PrizesTaken?.Invoke(prizes.Cast<ICard>().ToList());
            return prizes;
        }

        public override void RemoveCards(List<ICardLogic> cards)
        {
            base.RemoveCards(cards);
            PrizesTaken?.Invoke(cards.Cast<ICard>().ToList());
        }

        public override void DeckSearched()
        {
            if (_known)
                return;
            foreach (var card in Cards)
            {
                card.OwnerPositionKnowledge = PositionKnowledge.Known;
            }
            _known = true;
            BecameKnown?.Invoke();
        }
    }
}
