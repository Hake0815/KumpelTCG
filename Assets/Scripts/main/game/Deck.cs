using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;
using UnityEngine;

namespace gamecore.game
{
    public interface IDeck : ICardList
    {
        event Action<List<ICard>> CardsDrawn;
        event Action<List<ICard>> CardsDrawnFaceDown;
        event Action<List<ICard>> CardsAdded;
    }

    internal interface IDeckLogic : IDeck, ICardListLogic
    {
        List<ICardLogic> DrawFaceDown(int amount);
        void RemoveFaceDown(List<ICardLogic> cards);
        List<ICardLogic> Draw(int amount);
    }

    class Deck : IDeckLogic
    {
        public List<ICardLogic> Cards { get; set; }
        public int CardCount
        {
            get => Cards.Count;
        }

        public event Action CardCountChanged;
        public event Action<List<ICard>> CardsDrawn;
        public event Action<List<ICard>> CardsDrawnFaceDown;
        public event Action<List<ICard>> CardsAdded;

        public Deck(List<ICardLogic> cards)
        {
            Cards = cards;
        }

        public List<ICardLogic> DrawFaceDown(int amount)
        {
            var drawnCards = DrawCards(amount);
            OnCardsDrawnFaceDown(drawnCards);
            return drawnCards;
        }

        public List<ICardLogic> Draw(int amount)
        {
            var drawnCards = DrawCards(amount);
            OnCardsDrawn(drawnCards);
            return drawnCards;
        }

        private List<ICardLogic> DrawCards(int amount)
        {
            var drawnCards = Cards.GetRange(0, Math.Min(amount, Cards.Count));
            Cards.RemoveRange(0, drawnCards.Count);
            OnCardCountChanged();
            return drawnCards;
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
            OnCardsDrawn(cards);
            OnCardCountChanged();
        }

        public void RemoveCard(ICardLogic card)
        {
            Cards.Remove(card);
            OnCardsDrawn(new() { card });
            OnCardCountChanged();
        }

        public void RemoveFaceDown(List<ICardLogic> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardsDrawnFaceDown(cards);
            OnCardCountChanged();
        }

        public void OnCardCountChanged()
        {
            CardCountChanged?.Invoke();
        }

        public void OnCardsDrawn(List<ICardLogic> cards)
        {
            CardsDrawn?.Invoke(cards.Cast<ICard>().ToList());
        }

        public void OnCardsDrawnFaceDown(List<ICardLogic> cards)
        {
            CardsDrawnFaceDown?.Invoke(cards.Cast<ICard>().ToList());
        }

        public void OnCardsAdded(List<ICardLogic> cards)
        {
            CardsAdded?.Invoke(cards.Cast<ICard>().ToList());
        }
    }
}
