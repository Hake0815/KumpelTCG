using System;
using gamecore.serialization;

namespace gamecore.card
{
    public enum CardType
    {
        Unknown,
        Pokemon,
        Trainer,
        Energy,
    }

    public static class CardTypeToProtoBufExtensions
    {
        public static ProtoBufCardType ToProtoBuf(this CardType cardType)
        {
            return cardType switch
            {
                CardType.Unknown => ProtoBufCardType.CardTypeUnknown,
                CardType.Pokemon => ProtoBufCardType.CardTypePokemon,
                CardType.Trainer => ProtoBufCardType.CardTypeTrainer,
                CardType.Energy => ProtoBufCardType.CardTypeEnergy,
                _ => throw new InvalidOperationException($"Invalid card type: {cardType}"),
            };
        }
    }
}
