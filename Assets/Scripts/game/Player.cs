using System.Collections.Generic;

public class Player
{
    public IDeck Deck { get; }
    public List<Card> Hand { get; } = new();

    public Player(IDeck deck)
    {
        this.Deck = deck;
    }

    public void Draw()
    {
        Hand.Add(Deck.Draw());
    }
}
