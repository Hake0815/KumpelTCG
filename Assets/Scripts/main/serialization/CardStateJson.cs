using System;
using gamecore.card;

namespace gamecore.serialization
{
    [Serializable]
    public class CardStateJson : IJsonStringSerializable
    {
        public CardJson Card { get; }
        public PositionJson Position { get; }

        public CardStateJson(CardJson card, PositionJson position)
        {
            Card = card ?? throw new ArgumentNullException(nameof(card));
            Position = position ?? throw new ArgumentNullException(nameof(position));
        }
    }
}
