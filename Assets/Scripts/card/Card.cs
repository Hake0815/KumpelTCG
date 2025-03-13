using UnityEngine;

public abstract class Card
{
    public CardData CardData { get; private set; }

    public string Name
    {
        get => CardData.Name;
    }

    protected Card(CardData cardData)
    {
        CardData = cardData;
    }
}

public class CardDummy : Card
{
    public CardDummy(CardData cardData)
        : base(cardData) { }
}
