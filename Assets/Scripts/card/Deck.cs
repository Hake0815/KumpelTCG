using System.Collections.Generic;

public class Deck
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
