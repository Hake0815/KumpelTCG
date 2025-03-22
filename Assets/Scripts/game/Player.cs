using System;
using System.Collections.Generic;
using gamecore.card;

namespace gamecore.game
{
    public interface IPlayer
    {
        public IDeck Deck { get; }
        public IHand Hand { get; }
        public IDiscardPile DiscardPile { get; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public void Draw(int amount);
    }

    public class Player : IPlayer
    {
        public IDeck Deck { get; }
        public IHand Hand { get; } = new Hand();
        public IDiscardPile DiscardPile { get; } = new DiscardPile();
        public bool IsActive { get; set; } = false;
        public string Name { get; set; }

        public Player(IDeck deck)
        {
            Deck = deck;
        }

        public void Draw(int amount)
        {
            var drawnCards = Deck.Draw(amount);
            if (drawnCards != null)
            {
                Hand.AddCards(drawnCards);
            }
        }
    }
}
