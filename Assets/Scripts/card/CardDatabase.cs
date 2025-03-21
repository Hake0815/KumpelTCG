using System.Collections.Generic;
using gamecore.effect;

namespace gamecore.card
{
    public class CardDatabase
    {
        public static List<CardData> cardDataList = new()
        {
            new CardDataDummy(
                "first draw 2 Card",
                new List<IEffect> { new DrawCardsEffect(2), new DiscardCardEffect() }
            ),
        };
    }
}
