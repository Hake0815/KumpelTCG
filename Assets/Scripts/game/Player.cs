using System;
using System.Collections.Generic;
using gamecore.card;
using UnityEngine;

namespace gamecore.game
{
    public interface IPlayer
    {
        public IDeck Deck { get; }
        public List<ICard> Hand { get; }
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public void Draw();
        public event EventHandler<List<ICard>> CardDrawn;
    }

    public class Player : IPlayer
    {
        public IDeck Deck { get; }
        public List<ICard> Hand { get; } = new();
        public bool IsActive { get; set; } = false;
        public string Name { get; set; }

        public event EventHandler<List<ICard>> CardDrawn;

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
                OnCardDrawn(new List<ICard> { drawnCard });
            }
        }

        protected virtual void OnCardDrawn(List<ICard> drawnCards)
        {
            CardDrawn?.Invoke(this, drawnCards);
        }
    }
}
