using System;
using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game
{
    public interface IPlayer
    {
        public IDeck Deck { get; }
        public List<ICard> Hand { get; }
        public List<ICard> DiscardPile { get; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public void Draw();
        public void RemoveCardFromHand(ICard card);
        public event EventHandler<List<ICard>> CardsAddedToHand;
        public event Action CardsRemovedFromHand;
    }

    public class Player : IPlayer
    {
        public IDeck Deck { get; }
        public List<ICard> Hand { get; } = new();
        public List<ICard> DiscardPile { get; } = new();
        public bool IsActive { get; set; } = false;
        public string Name { get; set; }

        public event EventHandler<List<ICard>> CardsAddedToHand;
        public event Action CardsRemovedFromHand;

        public Player(IDeck deck)
        {
            Deck = deck;
        }

        public void Draw()
        {
            var drawnCard = Deck.Draw();
            if (drawnCard != null)
            {
                Hand.Add(drawnCard);
                OnCardsAddedToHand(new List<ICard> { drawnCard });
            }
        }

        protected virtual void OnCardsAddedToHand(List<ICard> drawnCards)
        {
            CardsAddedToHand?.Invoke(this, drawnCards);
        }

        public void RemoveCardFromHand(ICard card)
        {
            Hand.Remove(card);
            CardsRemovedFromHand?.Invoke();
        }
    }
}
