using System;
using System.Collections.Generic;

namespace gamecore.card
{
    public interface IHand : ICardList { }

    internal abstract class HandLogicAbstract : CardListLogicAbstract, IHand
    {
        protected HandLogicAbstract()
            : base(new()) { }
    }

    class Hand : HandLogicAbstract
    {
        public Hand()
            : base() { }

        public override event Action<List<ICard>> CardCountChanged;

        public override void OnCardCountChanged()
        {
            CardCountChanged?.Invoke(((ICardList)this).Cards);
        }

        public override void AddCards(List<ICardLogic> cards)
        {
            foreach (var card in cards)
            {
                card.OwnerPositionKnowledge = PositionKnowledge.Known;
            }
            base.AddCards(cards);
        }
    }
}
