using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;

namespace gamecore.game
{
    public interface IDiscardPile : ICardList
    {
        ICard LastCard { get; }
        event Action<List<ICard>> CardsAdded;
        event Action<List<ICard>> CardsRemoved;
    }

    internal abstract class DiscardPileLogicAbstract : CardListLogicAbstract, IDiscardPile
    {
        protected DiscardPileLogicAbstract()
            : base(new()) { }

        public ICard LastCard => Cards.LastOrDefault();

        public abstract event Action<List<ICard>> CardsAdded;
        public abstract event Action<List<ICard>> CardsRemoved;
    }

    class DiscardPile : DiscardPileLogicAbstract
    {
        public override event Action<List<ICard>> CardCountChanged;
        public override event Action<List<ICard>> CardsAdded;
        public override event Action<List<ICard>> CardsRemoved;

        public DiscardPile()
            : base() { }

        public override void AddCards(List<ICardLogic> cards)
        {
            foreach (var card in cards)
            {
                card.OwnerPositionKnowledge = PositionKnowledge.Known;
                card.OpponentPositionKnowledge = PositionKnowledge.Known;
            }
            base.AddCards(cards);
            OnCardsAdded(cards);
        }

        public override void RemoveCards(List<ICardLogic> cards)
        {
            base.RemoveCards(cards);
            OnCardsRemoved(cards);
        }

        public override void RemoveCard(ICardLogic card)
        {
            base.RemoveCard(card);
            OnCardsRemoved(new() { card });
        }

        public override void OnCardCountChanged()
        {
            CardCountChanged?.Invoke(((ICardList)this).Cards);
        }

        private void OnCardsAdded(List<ICardLogic> cards)
        {
            CardsAdded?.Invoke(cards.Cast<ICard>().ToList());
        }

        private void OnCardsRemoved(List<ICardLogic> cards)
        {
            CardsRemoved?.Invoke(cards.Cast<ICard>().ToList());
        }
    }
}
