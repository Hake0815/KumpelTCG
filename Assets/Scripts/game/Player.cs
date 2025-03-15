using System;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    public IDeck Deck { get; }
    public List<ICard> Hand { get; }
    public bool IsActive { get; set; }
    public void Draw();
    public event EventHandler<List<ICard>> CardDrawn;
}

public class Player : IPlayer
{
    public IDeck Deck { get; }
    public List<ICard> Hand { get; } = new();
    public bool IsActive { get; set; } = false;

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
        Debug.Log("Card Drawn, throw Event");
        CardDrawn?.Invoke(this, drawnCards);
    }
}
