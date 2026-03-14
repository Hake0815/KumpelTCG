using System;
using gamecore.serialization;

namespace gamecore.card
{
    public enum CardSubtype
    {
        Unknown,
        BasicPokemon,
        Stage1Pokemon,
        Stage2Pokemon,
        Supporter,
        Item,
        Tool,
        Stadium,
        BasicEnergy,
        SpecialEnergy,
    }

    public static class CardSubtypeToProtoBufExtensions
    {
        public static ProtoBufCardSubtype ToProtoBuf(this CardSubtype cardSubtype)
        {
            return cardSubtype switch
            {
                CardSubtype.Unknown => ProtoBufCardSubtype.CardSubtypeUnknown,
                CardSubtype.BasicPokemon => ProtoBufCardSubtype.CardSubtypeBasicPokemon,
                CardSubtype.Stage1Pokemon => ProtoBufCardSubtype.CardSubtypeStage1Pokemon,
                CardSubtype.Stage2Pokemon => ProtoBufCardSubtype.CardSubtypeStage2Pokemon,
                CardSubtype.Supporter => ProtoBufCardSubtype.CardSubtypeSupporter,
                CardSubtype.Item => ProtoBufCardSubtype.CardSubtypeItem,
                CardSubtype.Tool => ProtoBufCardSubtype.CardSubtypeTool,
                CardSubtype.Stadium => ProtoBufCardSubtype.CardSubtypeStadium,
                CardSubtype.BasicEnergy => ProtoBufCardSubtype.CardSubtypeBasicEnergy,
                CardSubtype.SpecialEnergy => ProtoBufCardSubtype.CardSubtypeSpecialEnergy,
                _ => throw new InvalidOperationException($"Invalid card subtype: {cardSubtype}"),
            };
        }
    }
}
