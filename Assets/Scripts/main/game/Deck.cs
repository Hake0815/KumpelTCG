using System;
using System.Collections.Generic;
using System.Linq;
using gamecore.card;

namespace gamecore.game
{
    public interface IDeck : ICardList
    {
        event Action<List<ICard>> CardsDrawn;
        event Action<List<ICard>> CardsDrawnFaceDown;
        event Action<List<ICard>> CardsAdded;
    }

    internal abstract class DeckLogicAbstract : CardListLogicAbstract, IDeck
    {
        protected DeckLogicAbstract(List<ICardLogic> cards)
            : base(cards) { }

        public abstract event Action<List<ICard>> CardsDrawn;
        public abstract event Action<List<ICard>> CardsDrawnFaceDown;
        public abstract event Action<List<ICard>> CardsAdded;

        public abstract List<ICardLogic> DrawFaceDown(int amount);
        public abstract void RemoveFaceDown(List<ICardLogic> cards);
        public abstract List<ICardLogic> Draw(int amount);
    }

    class Deck : DeckLogicAbstract
    {
        public override event Action<List<ICard>> CardCountChanged;
        public override event Action<List<ICard>> CardsDrawn;
        public override event Action<List<ICard>> CardsDrawnFaceDown;
        public override event Action<List<ICard>> CardsAdded;

        public Deck(List<ICardLogic> cards)
            : base(cards) { }

        public override List<ICardLogic> DrawFaceDown(int amount)
        {
            var drawnCards = DrawCards(amount);
            OnCardsDrawnFaceDown(drawnCards);
            return drawnCards;
        }

        public override List<ICardLogic> Draw(int amount)
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

        public override void Shuffle()
        {
            var n = CardCount;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (Cards[n], Cards[k]) = (Cards[k], Cards[n]);
                Cards[n].TopDeckPositionIndex = 0;
            }
            Cards[0].TopDeckPositionIndex = 0;
        }

        public override void AddCards(List<ICardLogic> cards)
        {
            foreach (var card in cards)
            {
                card.TopDeckPositionIndex = CardCount;
            }
            Cards.AddRange(cards);
            OnCardsAdded(cards);
            OnCardCountChanged();
        }

        public override void RemoveCards(List<ICardLogic> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardsDrawn(cards);
            OnCardCountChanged();
        }

        public override void RemoveCard(ICardLogic card)
        {
            Cards.Remove(card);
            OnCardsDrawn(new() { card });
            OnCardCountChanged();
        }

        public override void RemoveFaceDown(List<ICardLogic> cards)
        {
            Cards.RemoveAll(cards.Contains);
            OnCardsDrawnFaceDown(cards);
            OnCardCountChanged();
        }

        public override void OnCardCountChanged()
        {
            CardCountChanged?.Invoke(((ICardList)this).Cards);
        }

        public void OnCardsDrawn(List<ICardLogic> cards)
        {
            MoveCardsTopIndexUp(cards);
            CardsDrawn?.Invoke(cards.Cast<ICard>().ToList());
        }

        public void OnCardsDrawnFaceDown(List<ICardLogic> cards)
        {
            MoveCardsTopIndexUp(cards);
            CardsDrawnFaceDown?.Invoke(cards.Cast<ICard>().ToList());
        }

        private void MoveCardsTopIndexUp(List<ICardLogic> cards)
        {
            var otherCardCouldHaveBeenDrawn = false;
            foreach (var card in Cards)
            {
                int lowest = card.TopDeckPositionIndex - cards.Count;
                if (lowest < 0)
                {
                    otherCardCouldHaveBeenDrawn = true;
                    card.OpponentPositionKnowledge =
                        card.OpponentPositionKnowledge.InformationLost();
                }

                card.TopDeckPositionIndex = Math.Max(0, lowest);
            }
            if (otherCardCouldHaveBeenDrawn)
            {
                foreach (var card in cards)
                {
                    card.OpponentPositionKnowledge =
                        card.OpponentPositionKnowledge.InformationLost();
                    card.TopDeckPositionIndex = 0;
                }
            }
            else
            {
                foreach (var card in cards)
                {
                    card.TopDeckPositionIndex = 0;
                }
            }
        }

        public void OnCardsAdded(List<ICardLogic> cards)
        {
            CardsAdded?.Invoke(cards.Cast<ICard>().ToList());
        }
    }
}
