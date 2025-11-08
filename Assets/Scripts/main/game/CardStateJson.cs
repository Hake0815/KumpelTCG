using System;
using gamecore.card;
using gamecore.common;

namespace gamecore.game
{
    [Serializable]
    public class CardStateJson : JsonStringSerializable
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
