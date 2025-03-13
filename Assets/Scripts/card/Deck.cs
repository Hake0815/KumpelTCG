using System.Collections.Generic;

public interface IDeck
{
    public void SetUp(List<Card> cards);
    public Card Draw();
    public int GetCardCount();
}

public class Deck : IDeck
{
    private List<Card> Cards { get; set; }

    public void SetUp(List<Card> cards)
    {
        Cards = cards;
    }

    public Card Draw()
    {
        if (Cards?.Count > 0)
        {
            var drawnCard = Cards[0];
            Cards.RemoveAt(0);
            return drawnCard;
        }
        return null;
    }

    public int GetCardCount()
    {
        return Cards.Count;
    }
}
