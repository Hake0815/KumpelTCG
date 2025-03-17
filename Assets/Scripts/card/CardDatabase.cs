using System.Collections.Generic;

namespace gamecore.card
{
    public class CardDatabase
    {
        public static List<CardData> cardDataList = new()
        {
            new CardDataDummy("first Card"),
            new CardDataDummy("second Card"),
            new CardDataDummy("third Card"),
        };
    }
}
