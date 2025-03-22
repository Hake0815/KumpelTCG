using System.Collections.Generic;
using gamecore.effect;

namespace gamecore.card
{
    public class CardDatabase
    {
        public static List<ICardData> cardDataList = new()
        {
            new TrainerCardData(
                "Bill",
                "bill",
                new List<IEffect> { new DrawCardsEffect(2), new DiscardCardEffect() },
                new List<IPlayCondition> { new HasCardsInDeck() }
            ),
        };
    }
}
