using UnityEngine;

public interface ICard
{
    public CardData CardData { get; set; }

    public string Name
    {
        get => CardData.Name;
    }
}

public class CardDummy : ICard
{
    public CardData CardData { get; set; }

    public CardDummy(CardData cardData)
    {
        CardData = cardData;
    }

    public static ICard Of(CardData cardData)
    {
        return new CardDummy(cardData);
    }
}
