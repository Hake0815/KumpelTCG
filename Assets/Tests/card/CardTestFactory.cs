public class CardTestFactory
{
    public static CardData CARD_DATA = CardDataTestFactory.Create();

    public static ICard Create()
    {
        return new CardDummy(CARD_DATA);
    }
}
