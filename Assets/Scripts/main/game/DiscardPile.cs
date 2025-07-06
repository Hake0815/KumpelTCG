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

    internal interface IDiscardPileLogic : IDiscardPile, ICardListLogic { }

    class DiscardPile : IDiscardPileLogic
    {
        public List<ICardLogic> Cards { get; } = new();
        public event Action<List<ICard>> CardCountChanged;
        public event Action<List<ICard>> CardsAdded;
        public event Action<List<ICard>> CardsRemoved;

        public int CardCount
        {
            get => Cards.Count;
        }

        public ICard LastCard
        {
            get => Cards.LastOrDefault();
        }

        public void AddCards(List<ICardLogic> cards)
        {
            Cards.AddRange(cards);
            OnCardsAdded(cards);
            OnCardCountChanged();
        }

        public void RemoveCards(List<ICardLogic> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardsRemoved(cards);
            OnCardCountChanged();
        }

        public void RemoveCard(ICardLogic card)
        {
            Cards.Remove(card);
            OnCardsRemoved(new() { card });
            OnCardCountChanged();
        }

        public void OnCardCountChanged()
        {
            CardCountChanged?.Invoke(((ICardList)this).Cards);
        }

        public void OnCardsAdded(List<ICardLogic> cards)
        {
            CardsAdded?.Invoke(cards.Cast<ICard>().ToList());
        }

        public void OnCardsRemoved(List<ICardLogic> cards)
        {
            CardsRemoved?.Invoke(cards.Cast<ICard>().ToList());
        }
    }
}
