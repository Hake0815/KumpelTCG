using UnityEngine;

namespace gamecore.card
{
    public class CardDataTestFactory
    {
        public const string NAME = "TestName";

        public static CardData Create()
        {
            return new CardDataDummy(NAME);
        }
    }
}
